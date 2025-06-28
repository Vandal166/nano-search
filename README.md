# nano-search

Why not use the NTFS Master File Table (MFT) to scan?
- it requires admin privileges
- works only for NTFS
- hard p/invoke implementatino ;C
- would need to fallback to standard win32 if NTFS is not available
