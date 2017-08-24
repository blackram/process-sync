# process-sync
prototype code for synching across processes

# Named Mutex
A simple utility using a named mutex to synchronise processes on the same machine. Console output clearly shows blocking.

# File Locking
Simple utility using the FileLock nuGet from [markedup-mobi/file-lock](https://github.com/markedup-mobi/file-lock). Works across machines

# Work sharing
An example of a simple task being performed by two processes that is synchronised using the file lock library. The example work is to read a counter
from a file, update the counter file and then write a file name to an output folder with the counter as the key (filename). No file collisions
occurred during testing

