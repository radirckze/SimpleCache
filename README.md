# SimpleCache

This is my testbed for a simple, synchronized cache. The cache allows for multiple readers. When there is a cache miss and the cache is being updated, readers will be blocked. 

This is a Visual Studio Code / C# project

[TODO] When cache is full, the cache will pick a random object to evict. Need to add a reasonable eviction policy.

[TODO] Objects in the cache do not expire. Need to add an expire policy. Better yet, need to add hooks so the can can be informed via either the DB or the application layer (which may be difficult to do).
