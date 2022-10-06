
<!DOCTYPE html>
<!--
  vim: set sts=2 sw=2 et :


  Demo Javascript app for negotiating and streaming a sendrecv webrtc stream
  with a GStreamer app. Runs only in passive mode, i.e., responds to offers
  with answers, exchanges ICE candidates, and streams.

  Author: Nirbheek Chauhan <nirbheek@centricular.com>
-->
<html>
  <head>
    <meta charset="utf-8"/>
    <style>
      .error { color: red; }
    </style>
    <script src="https://webrtc.github.io/adapter/adapter-latest.js"></script>
    <script>/* vim: set sts=4 sw=4 et :
      *
      * Demo Javascript app for negotiating and streaming a sendrecv webrtc stream
      * with a GStreamer app. Runs only in passive mode, i.e., responds to offers
      * with answers, exchanges ICE candidates, and streams.
      *
      * Author: Nirbheek Chauhan <nirbheek@centricular.com>
      */
     
     // Set this to override the automatic detection in websocketServerConnect()
     
     var ws_server = "192.168.136.152";
     var ws_port;
     // Set this to use a specific peer id instead of a random one
     var default_peer_id;
     // Override with your own STUN servers if you want
     var rtc_configuration = {iceServers: [{urls: "stun:stun.services.mozilla.com"},
                                           {urls: "stun:stun.l.google.com:19302"}]};
     // The default constraints that will be attempted. Can be overriden by the user.
     var default_constraints = {video: false, audio: false};
     
     var connect_attempts = 0;
     var peer_connection;
     var send_channel;
     var ws_conn;
     
     
     function setConnectButtonState(value) {
         document.getElementById("peer-connect-button").value = value;
     }
     
     function wantRemoteOfferer() {
        return document.getElementById("remote-offerer").checked;
     }
     
     function onConnectClicked() {
         if (document.getElementById("peer-connect-button").value == "Disconnect") {
             resetState();
             return;
         }
     
         var id = document.getElementById("peer-connect").value;
         if (id == "") {
             alert("Peer id must be filled out");
             return;
         }
     
         ws_conn.send("SESSION " + id);
         setConnectButtonState("Disconnect");
     }
     
     function resetState() {
         // This will call onServerClose()
         ws_conn.close();
     }
     
     function handleIncomingError(error) {
         setError("ERROR: " + error);
         resetState();
     }
     
     function getVideoElement() {
         return document.getElementById("stream");
     }
     
     function setStatus(text) {
         console.log(text);
         var span = document.getElementById("status")
         // Don't set the status if it already contains an error
         if (!span.classList.contains('error'))
             span.textContent = text;
     }
     
     function setError(text) {
         console.error(text);
         var span = document.getElementById("status")
         span.textContent = text;
         span.classList.add('error');
     }
     
     function resetVideo() {
         // Reset the video element and stop showing the last received frame
         var videoElement = getVideoElement();
         videoElement.pause();
         videoElement.src = "";
         videoElement.load();
     }
     
     // SDP offer received from peer, set remote description and create an answer
     function onIncomingSDP(sdp) {
         peer_connection.setRemoteDescription(sdp).then(() => {
             setStatus("Remote SDP set");
             if (sdp.type != "offer")
                 return;
             setStatus("Got SDP offer");
             peer_connection.createAnswer()
             .then(onLocalDescription).catch(setError);
         }).catch(setError);
     }
     
     // Local description was set, send it to peer
     function onLocalDescription(desc) {
         console.log("Got local description: " + JSON.stringify(desc));
         peer_connection.setLocalDescription(desc).then(function() {
             setStatus("Sending SDP " + desc.type);
             sdp = {'sdp': peer_connection.localDescription}
             ws_conn.send(JSON.stringify(sdp));
         });
     }
     
     function generateOffer() {
         peer_connection.createOffer().then(onLocalDescription).catch(setError);
     }
     
     // ICE candidate received from peer, add it to the peer connection
     function onIncomingICE(ice) {
         var candidate = new RTCIceCandidate(ice);
         peer_connection.addIceCandidate(candidate).catch(setError);
     }
     
     function onServerMessage(event) {
         console.log("Received " + event.data);
       
       //get peer id from signalling server, and show it.
       if (event.data.startsWith("HELLO"))
       {
         const data = event.data.split(" ");
         const hard_coded_id = data[1];
         setStatus("Registered with server, waiting for call");
         document.getElementById("peer-id").textContent = hard_coded_id;
             return;
       }
       
         switch (event.data) {
             case "SESSION_OK":
                 setStatus("Starting negotiation");
                 if (wantRemoteOfferer()) {
                     ws_conn.send("OFFER_REQUEST");
                     setStatus("Sent OFFER_REQUEST, waiting for offer");
                     return;
                 }
                 if (!peer_connection)
                     createCall(null).then (generateOffer);
                 return;
             case "OFFER_REQUEST":
                 // The peer wants us to set up and then send an offer
                 if (!peer_connection)
                     createCall(null).then (generateOffer);
                 return;
             default:
                 if (event.data.startsWith("ERROR")) {
                     handleIncomingError(event.data);
                     return;
                 }
                 // Handle incoming JSON SDP and ICE messages
                 try {
                     msg = JSON.parse(event.data);
                 } catch (e) {
                     if (e instanceof SyntaxError) {
                         handleIncomingError("Error parsing incoming JSON: " + event.data);
                     } else {
                         handleIncomingError("Unknown error parsing response: " + event.data);
                     }
                     return;
                 }
     
                 // Incoming JSON signals the beginning of a call
                 if (!peer_connection)
                     createCall(msg);
     
                 if (msg.sdp != null) {
                     onIncomingSDP(msg.sdp);
                 } else if (msg.ice != null) {
                     onIncomingICE(msg.ice);
                 } else {
                     handleIncomingError("Unknown incoming JSON: " + msg);
                 }
         }
     }
     
     function onServerClose(event) {
         setStatus('Disconnected from server');
         resetVideo();
     
         if (peer_connection) {
             peer_connection.close();
             peer_connection = null;
         }
     
         // Reset after a second
         window.setTimeout(websocketServerConnect, 1000);
     }
     
     function onServerError(event) {
         setError("Unable to connect to server, did you add an exception for the certificate?")
         // Retry after 3 seconds
         window.setTimeout(websocketServerConnect, 3000);
     }
     
     function websocketServerConnect() {
         connect_attempts++;
         if (connect_attempts > 3) {
             setError("Too many connection attempts, aborting. Refresh page to try again");
             return;
         }
         // Clear errors in the status span
         var span = document.getElementById("status");
         span.classList.remove('error');
         span.textContent = '';
       
         ws_port = ws_port || '8443';
         if (window.location.protocol.startsWith ("file")) {
             ws_server = ws_server || "127.0.0.1";
         } else if (window.location.protocol.startsWith ("http")) {
             ws_server = ws_server || window.location.hostname;
         } else {
             throw new Error ("Don't know how to connect to the signalling server with uri" + window.location);
         }
         var ws_url = 'wss://' + ws_server + ':' + ws_port
         setStatus("Connecting to server " + ws_url);
         ws_conn = new WebSocket(ws_url);
         /* When connected, immediately register with the server */
         ws_conn.addEventListener('open', (event) => {	
       
         //sending signalling server "HELLO" and wait for id.
             ws_conn.send('HELLO');
             setStatus("Registering with server");
             setConnectButtonState("Connect");
         });
         ws_conn.addEventListener('error', onServerError);
         ws_conn.addEventListener('message', onServerMessage);
         ws_conn.addEventListener('close', onServerClose);
     }
     
     function onRemoteTrack(event) {
         if (getVideoElement().srcObject !== event.streams[0]) {
             console.log('Incoming stream');
             getVideoElement().srcObject = event.streams[0];
         }
     }
     
     const handleDataChannelOpen = (event) =>{
         console.log("dataChannel.OnOpen", event);
     };
     
     const handleDataChannelMessageReceived = (event) =>{
         console.log("dataChannel.OnMessage:", event, event.data.type);
     
         setStatus("Received data channel message");
         if (typeof event.data === 'string' || event.data instanceof String) {
             console.log('Incoming string message: ' + event.data);
             textarea = document.getElementById("text")
             textarea.value = textarea.value + '\n' + event.data
         } else {
             console.log('Incoming data message');
         }
         send_channel.send("Hi! (from browser)");
     };
     
     const handleDataChannelError = (error) =>{
         console.log("dataChannel.OnError:", error);
     };
     
     const handleDataChannelClose = (event) =>{
         console.log("dataChannel.OnClose", event);
     };
     
     function onDataChannel(event) {
         setStatus("Data channel created");
         let receiveChannel = event.channel;
         receiveChannel.onopen = handleDataChannelOpen;
         receiveChannel.onmessage = handleDataChannelMessageReceived;
         receiveChannel.onerror = handleDataChannelError;
         receiveChannel.onclose = handleDataChannelClose;
     }
     
     function createCall(msg) {
         // Reset connection attempts because we connected successfully
         connect_attempts = 0;
     
         console.log('Creating RTCPeerConnection');
     
         peer_connection = new RTCPeerConnection(rtc_configuration);
         send_channel = peer_connection.createDataChannel('label', null);
         send_channel.onopen = handleDataChannelOpen;
         send_channel.onmessage = handleDataChannelMessageReceived;
         send_channel.onerror = handleDataChannelError;
         send_channel.onclose = handleDataChannelClose;
         peer_connection.ondatachannel = onDataChannel;
         peer_connection.ontrack = onRemoteTrack;
     
         if (msg != null && !msg.sdp) {
             console.log("WARNING: First message wasn't an SDP message!?");
         }
     
         peer_connection.onicecandidate = (event) => {
             // We have a candidate, send it to the remote party with the
             // same uuid
             if (event.candidate == null) {
                     console.log("ICE Candidate was null, done");
                     return;
             }
             ws_conn.send(JSON.stringify({'ice': event.candidate}));
         };
     
         if (msg != null)
             setStatus("Created peer connection for call, waiting for SDP");
     
         setConnectButtonState("Disconnect");
     }</script>
    <script>
	  window.onload = websocketServerConnect;
    </script>
  </head>

  <body>
    <div>
    <video id="stream" autoplay muted playsinline>Your browser doesn't support video</video>
    </div>
    <div>Status: <span id="status">unknown</span></div>
    <div><textarea id="text" cols=40 rows=4></textarea></div> 
    <br/>
    <div>
      <label for="peer-connect">Enter peer id</label>
      <input id="peer-connect" type="text" name="text">
      <input id="peer-connect-button" onclick="onConnectClicked();" type="button" value="Connect">
      <!-- Request the peer to send the offer by sending the OFFER_REQUEST message.
        Same as the -​-remote-offerer flag in the sendrecv C example -->
      <input id="remote-offerer" type="checkbox" autocomplete="off"><span>Remote offerer</span>
    </div>

    <div>Our id is <b id="peer-id">unknown</b></div>
  </body>
</html>
