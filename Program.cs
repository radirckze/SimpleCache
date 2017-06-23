using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleCache {

    class Program {

        static void Main(string[] args) {

            Console.WriteLine("Testing simple cache ...");

            MemberDb memberDb = new MemberDb();
            InitDatabase(memberDb);
            MemberCache memberCache = new MemberCache(memberDb);
            List<Task> tasks = new List<Task>();

            //Start a task to add new members. Adds a new member every .5 secons for 5 secons
            Task task = new Task(() => AddNewMemberTask(memberDb, GetNewMemberList(), 5000, 500));
            task.Start();
            tasks.Add(task);

            //Start a task to delete random members. Deletes a member with id betwween
            //5-20 every .5 seconds for 5 seconds. Note: some id's may not be valid.
            task = new Task( () => DeleteMemberTask(memberDb, 5000, 500, 6, 15));
            task.Start();
            tasks.Add(task);

            //Create 2 tasks that read random ids between 1-7 every 250 ms for 5 seconds.
            task = new Task(() => CacheReader(memberCache, 5000, 250, 8));
            task.Start();
            tasks.Add(task);
            task = new Task(() => CacheReader(memberCache, 5000, 250, 8));
            task.Start();
            tasks.Add(task);

            //Create a task that read random ids between 1-20 every 250 ms for 5 seconds.
            //Note some requested Ids may not be valid
            task = new Task(() => CacheReader(memberCache, 5000, 250, 20));
            task.Start();
            tasks.Add(task);

            Task.WaitAll(tasks.ToArray());


            memberCache.PrintCache();
            memberDb.PrintDb();



        }

        //timespan - how long to run; interval - iverval between runs;
        // maxId - range of Ids to use in requests
        public static void CacheReader(MemberCache memberCache, int timespan, 
            int interval, int maxId) {
            int elapsed = 0;
            Random rand = new Random();
            int memId = 0;
            Member member = null;
            
            while(elapsed < timespan) {
                memId = rand.Next(1, maxId) ; 
                member = memberCache.GetMember(memId);
                if (member == null) {
                    Console.WriteLine("ProcId:{0} Cache.GetMember:{1} returned null", 
                        Environment.CurrentManagedThreadId, memId.ToString());
                }
                else {
                    Console.WriteLine("ProcId:{0} Cache.GetMember:{1} returned {2}", 
                        Environment.CurrentManagedThreadId, memId, member.ToString());
                }

                Thread.Sleep(interval);
                elapsed = elapsed + interval;
            }

        }

        public static void AddNewMemberTask(MemberDb memberDb, List<Member> newMembers, 
            int timespan, int interval) {

             int run = 0;
             int elapsed = 0;
             while(elapsed < timespan && run < newMembers.Count) {
                memberDb.AddMember(newMembers[run]); 
                run++;
                Thread.Sleep(interval);
                elapsed = elapsed + interval;
            }

        }

        public static void DeleteMemberTask(MemberDb memberDb, int timespan, int 
            interval, int minId, int maxId) {
            
            int elapsed = 0;
            Random rand = new Random();
            int memberId = 0;
            while(elapsed < timespan) {
                memberId = rand.Next(minId, maxId); //Note, Id may not be valid
                memberDb.RemoveMember(memberId);
                Thread.Sleep(interval);
                elapsed = elapsed + interval;
            }

        }

        // add some initial data.
        private static void InitDatabase(MemberDb memberDb) {

            memberDb.AddMember(new Member("Mia"));
            memberDb.AddMember(new Member("Vincent"));
            memberDb.AddMember(new Member("Jules"));
            memberDb.AddMember(new Member("Marsellus"));
            memberDb.AddMember(new Member("Winston"));
            memberDb.AddMember(new Member("Butch"));
             memberDb.AddMember(new Member("Jimmie"));
            memberDb.AddMember(new Member("Honey"));
            memberDb.AddMember(new Member("Pumpkin"));
            memberDb.AddMember(new Member("Koons"));
        }

        public static List<Member> GetNewMemberList() {

            List<Member> newMembers = new List<Member>();
            newMembers.Add(new Member("Esmarelda"));
            newMembers.Add(new Member("Zed"));
            newMembers.Add(new Member("Fabienne"));
            newMembers.Add(new Member("Bonnie"));
            newMembers.Add(new Member("Paul"));
            newMembers.Add(new Member("Buddy"));
            newMembers.Add(new Member("Lance"));
            newMembers.Add(new Member("Jody"));
            newMembers.Add(new Member("Maynard"));
            newMembers.Add(new Member("Marilyn"));

            return newMembers;

        }



    }
}
