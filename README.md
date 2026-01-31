# Edoire Tools

This toolkit is designed for modifying Unity games developed with the Edoire.

## Resource Packs

Resource pack files for this engine have the `.arc` extension, and their file header is `@ARCH000`.

### Extracting Files from a Resource Pack

Run the following command:
```
ArchiveTool -e -in input.arc -out Extract
```

Parameter Description:
- `-e`: Extract files from the resource pack.
- `-in`: Specify the resource pack filename.
- `-out`: Specify the directory where extracted items will be stored.

### Creating a Resource Pack

Run the following command:
```
ArchiveTool -c -in RootDirectory -out output.arc
```

Parameter Description:
- `-c`: Create a resource pack.
- `-in`: Specify the folder containing files you wish to add to the resource pack.
- `-out`: Specify the resource pack filename.

## Images

Run the following command:
```
DataPackTool -e -in evcg.bytes.unity3d -out Extract
```

---

**Note:** This toolkit has been tested on a limited number of games only.
