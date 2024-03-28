# Use Alpine Linux as the base image
FROM ghcr.io/osgeo/gdal:alpine-small-3.8.4

# Create a working directory
WORKDIR /app

# Set the entrypoint to ogr2ogr
ENTRYPOINT ["ogr2ogr"]

# Default command (override with docker run command)
CMD ["--help"]

# docker run -v "C:\Users\Arne\Downloads\gebouw_ALL":/app ogr2ogr -f "GPKG" output.gpkg gebouw_ALL.shp