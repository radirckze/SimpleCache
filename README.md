# SimpleCache

This is my tesbed for a simple, synchronized cache. The cache allows for multiple readers. When there is a cache miss and the cache is being updated, readers will be blocked. 

This is a Visual Studio Code / C# project

[TODO] When cache is full, the cache will pick a random object to evict. Need to add a reasonable eviction policy.
[TODO] For an object in the cache, if the object in the database is updated, the cache still holds onto its copy. I've added the hook but need to add a process that the a
application layer can call to invalidate the object.
