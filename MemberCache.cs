using System;
using System.Collections.Generic;
using System.Threading;

namespace SimpleCache { 

    //[TODO] Need to evict cache object when it has been updated / deteled in DB
    //[TODO] Provide a legit eviction policy when cache is full. This will require
    //a custom data strucutre, probably one for memberCache.

    public class MemberCache {

        private int cacheSize = 7;
        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private Dictionary<int, Member> memberCache = new Dictionary<int, Member>();
        private MemberDb memberDb = null;  //this is the database

        public MemberCache(MemberDb theDb) {
            if (theDb == null) {
                throw new ArgumentException();
            }

            this.memberDb = theDb;
        }

        // Gets member from cache. If not it cache, will load from DB into cache. 
        // when adding to the cache, if cache is full, will remove random element.    
        public Member GetMember(int id) {
            bool found = true;
            Member theMember = null;
            
            cacheLock.EnterReadLock();
            try {
                if (memberCache.ContainsKey(id)) {
                    theMember = memberCache[id].Clone();
                    Console.WriteLine("[cache]Member {0} was in cache", theMember.Id);
                }
                else {
                    found = false;
                }
            }
            finally {
                cacheLock.ExitReadLock();
            }

            //if not found, it could be a cache miss, or invalid id
            if (!found) {
                //using an upgradeable read lock as things could have happened between
                //releasing the read lock and obtaining the next lock.
                cacheLock.EnterUpgradeableReadLock();
                try {
                    Member dbMember = this.memberDb.GetMember(id);
                    if (dbMember!= null) {
                        if (!memberCache.ContainsKey(id)) {
                            cacheLock.EnterWriteLock();
                            try {
                                if (memberCache.Count >= cacheSize) {
                                    IEnumerator<int> enumerator= memberCache.Keys.GetEnumerator();
                                    enumerator.MoveNext();
                                    memberCache.Remove(enumerator.Current);
                                    Console.WriteLine("[cache]Cache is full. One member evicted");
                                }
                                memberCache.Add(dbMember.Id, dbMember);
                                theMember = dbMember.Clone();
                                Console.WriteLine("[cache]Member {0} was added to cache", theMember.Id);
                            }
                            finally {
                                cacheLock.ExitWriteLock();
                            }
                        }
                    }
                }
                finally {
                    cacheLock.ExitUpgradeableReadLock();
                }

            }

            return theMember;
        }

        
        //This is not used currently!
        //It is intended to be a hook so that the cache can be updated proactively
        //when DB is updated. Need to figure this part out. 
        public void RemoveMember(int id) {
            cacheLock.EnterWriteLock();
            try {
                if (memberCache.ContainsKey(id)) {
                    memberCache.Remove(id);
                }
            }
            finally {
                cacheLock.ExitWriteLock();
            }
        }

        public void PrintCache() {

            Console.WriteLine("\nPrinting cache content ...");
            if (memberCache != null) {
                foreach(Member member in memberCache.Values) {
                    Console.WriteLine(member.ToString());
                }
            }
        }

    }
}