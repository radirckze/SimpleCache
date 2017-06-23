using System;
using System.Collections.Generic;
using System.Threading;

namespace SimpleCache { 

    public class MemberDb {

        private Dictionary<int, Member> members = new Dictionary<int, Member>();
        private ReaderWriterLockSlim rwLockSlim = new ReaderWriterLockSlim();
        private int nextId = 1; //this is a member


        public MemberDb() {
            // currently nothing to do
        }

        public Member GetMember(int id) {
            rwLockSlim.EnterReadLock();
            try {
                if (members.ContainsKey(id)) {
                    return  members[id].Clone();
                }
                else {
                    return null;
                }
            }
            finally {
                rwLockSlim.ExitReadLock();
            }
        }

        public Member AddMember(Member member) {
            if (member != null) {
                rwLockSlim.EnterWriteLock();
                try {
                    member.Id = nextId++;
                    members.Add(member.Id, member.Clone());
                    Console.WriteLine("[db]New member {0} added", member.Id);
                }
                finally {
                    rwLockSlim.ExitWriteLock();
                }
            }

            return member;
        }

        public bool RemoveMember(int id) {
            bool status = false;
            rwLockSlim.EnterWriteLock();
            Console.WriteLine("[db]Request to remove {0}", id);
            try {
                if (members.ContainsKey(id)) {
                    members.Remove(id);
                    status = true;
                }
            }
            finally {
                rwLockSlim.ExitWriteLock();
            }

            return status;
        }


        //aux methods

        public void PrintDb() {
            rwLockSlim.EnterReadLock();
            try {
                Console.WriteLine("\nPrinting DB content ...");
                if (members != null) {
                    foreach(Member member in members.Values) {
                        Console.WriteLine(member.ToString());
                    }
                }
            }
            finally {
                rwLockSlim.ExitReadLock();
            }
        }

    }

}