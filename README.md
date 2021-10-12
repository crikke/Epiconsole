# Epiconsole
CLI for Episerver CMS


## About the project  
I created this because I had to migrate an existing episerver site to another CMS and couldn´t use the built-in exporter due to the massive  ammount of content, localized over 10 languages.
Also, since I didn´t have access to the codebase I couldn't create a scheduled job.

So the best approach was to create this standalone console application which uses EPiServerDB directly and create a hierarchical export

## Additional Info
If you are going to do something similar, this project maybe at least could provide you with some gotchas (example: Engine Initialization & Loading Required Assemblies, BlobProvider)


## TODO:
- Implement https://github.com/commandlineparser/commandline
- Move out export configuration or make it overridable by CLI parameters 
- Refactor export as a Verb
