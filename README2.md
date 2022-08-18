# 3d-offline-map

A guide of how to create 3d web map offline using Cesium and openlayers. 

## Installation

Clone the repository

### Tiles
Extract tiles.rar,
Includes:
  * medium resulotion tiles all over the world,

  * high resulotion tiles of israel

You can download your own tiles using Maperitive, and pbf files which you can find here http://download.geofabrik.de/index.html

### Terrain
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
