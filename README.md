PhotographySorter helps you centralize your image collection. It can combine multiple source directories into one or more target directories, optionally even constructing a human-navigable structure. Config files are simple flat files.

Usage typically follows this pattern:
1. Add source directories \
`directory-add -t source -d D:\Art -a dart` \
`directory-add -t source -d D:\Series\Photography -a photography`

2. Add a target directory \
`directory-add -t target -d \\my-nas\James nas`

3. Execute a move \
`exec-move -s dart -t nas`

Directories can contain unrelated directories, but they will be automatically included unless explicitly blacklisted or the directory is another entry in the targets/sources list.