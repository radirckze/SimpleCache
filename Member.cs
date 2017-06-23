using System;

namespace SimpleCache { 

    public class Member {

        public int Id {get; set;}

        public string Name {get; set;} 

        public Member(string name, int id=0) {
            if (String.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException();
            }
            
            this.Id = id;
            this.Name = name;
        }

        public Member Clone() {
            return (Member) this.MemberwiseClone();
        }

        public override string ToString() {
            return String.Format("Id= {0}, Name={1}", this.Id, this.Name);
        }

    }

}
  