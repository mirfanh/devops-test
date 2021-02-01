**docker-compose up -d** should do the job

Quick notes.

App is using file system watcher to process incoming files, to handle an exceptional case where a file creation event is triggered but the file is not yet ready,
There is a timer job running every 10 second to scan the directory to see if there are any pending files to be processed.

As there is an immediate requirement to aggregate results on L3 category, L3 category is stored in Products table as well to avoid a cross lookup.

ToDo: Implement Category table to store categories in more structure way.
