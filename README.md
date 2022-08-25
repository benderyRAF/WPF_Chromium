# WPF_CESIUM

## Website installation and setup:
#### Tiles - 
Extract tiles.rar,
Includes:
  * medium resulotion tiles all over the world,

  * high resulotion tiles of israel

You can download your own tiles using Maperitive, and pbf files which you can find here http://download.geofabrik.de/index.html

#### Terrain -
Extract terrain file.rar

Install docker for windows https://hub.docker.com/editions/community/docker-ce-desktop-windows

Then pull cesium terrain builder https://registry.hub.docker.com/r/homme/cesium-terrain-builder

In the command line: **`docker run -v  d:/gisdata/israel/terrain:/data -ti -i homme/cesium-terrain-builder:latest bash`** to start the tarrain builder

Put the extracted terrain file in the created data folder, and create directory called tiles, then start generating the tiles with: 
**`root@29a49b64dac3:/data# ctb-tile -o /data/tiles /data/30n030e_20101117_gmted_dsc075.tif`**                                 
It will take up to 10 minutes, it creates tiles for 11 zoom levels, the zoom level depands on the .tiff file

Upload the tiles to a local server using docker:
**`docker run -p 9002:8000 -v d:/gisdata/israel/terrain:/data/tilesets/terrain geodata/cesium-terrain-server`**

This terrain files is about a big part of israel, you can download your own .tiff files (just need to find them)

## Requirements
Due to google CORS policy that is blocked in modern browsers by default (in JavaScript APIs), this can't run on regular browser.
but it can easily be avoided by running it using visual code live server extension.
or by enabaling CORS (more complicated and might be dangerous)

## WPF project startup
Run the WPF project in Visual studio

## GST-WebRTC setup
In order for this part of the project to run, you have to download Gstreamer plugins and GStreamer WebRTC implementation.
Here are the commands for Ubuntu 18.04 : `sudo apt-get install -y gstreamer1.0-tools gstreamer1.0-nice gstreamer1.0-plugins-bad gstreamer1.0-plugins-ugly gstreamer1.0-plugins-good libgstreamer1.0-dev git libglib2.0-dev libgstreamer-plugins-bad1.0-dev libsoup2.4-dev libjson-glib-dev`

WebRTC works with a signalling server and peers:
![image](https://user-images.githubusercontent.com/88430393/185790335-9343797a-cb7b-4a6d-9b1a-252a6e66405b.png)

First, run my-gts-webrtc/signalling/generate_cert.sh . this script will generate a self-signed certificate for the signalling server.
Then run my-gts-webrtc/signalling/simple_server.py. This is the signalling server which establishes a connection between the peers.
every instance of my-gts-webrtc/js/index.html is a peer.
and my-gts-webrtc/webrtc-sendrecv.py is the second peer.

After running the WPF project in visual studio, ensure that the status of each html is "Registered with server, waiting for call", and note the id.
Then run my-gts-webrtc/webrtc-sendrecv.py with the address of the signalling server and the id, for example: `python3 webrtc-sendrecv.py --server wss://localhost:8443 YOUR-ID`

Note that you can use the demo page: https://webrtc.nirbheek.in/ for which you don't need a signalling server. in that case, run the script like that:
`python3 webrtc-sendrecv.py YOUR-ID`. You can change path to index.html and url of signalling server in MainWindow.xaml under Window.Resources tag.

Also Note that when running my-gts-webrtc/webrtc-sendrecv.py, signalling server must be accessed with WSS protocol.

For further reading and help, visit : https://gitlab.freedesktop.org/gstreamer/gst-examples/-/tree/master/webrtc. scripts under my-gts-webrtc/ was taken from that repo. 
And also gstreamer documentation: https://gstreamer.freedesktop.org/documentation/tutorials/index.html?gi-language=c
