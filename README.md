PhotographySorter helps you centralize your image collection. It can combine multiple source directories into one or more target directories, optionally even constructing a human-navigable structure. Config files are simple flat files.

Usage typically follows this pattern:
1. Add source directories
2. Add a target directory
3. Execute a move

Directories can contain unrelated directories, but they will be automatically included unless explicitly blacklisted or the directory is another entry in the targets/sources list.