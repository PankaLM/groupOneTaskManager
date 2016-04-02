using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Aggregates.Tasks
{
    public class State
    {
        public static State Initialized = new State(1, "Initialized");

        public static State InProgress = new State(2, "InProgress");

        public static State Done = new State(3, "Done");

        private State(int StateId, string name)
        {
            this.StateId = StateId;
            this.Name = name;
        }

        public int StateId { get; private set; }

        public string Name { get; private set; }

        public static State GetById(int id)
        {
            return GetAll().Where(e => e.StateId == id).SingleOrDefault();
        }

        public static IEnumerable<State> GetAll()
        {
            yield return InProgress;
            yield return Done;
            yield return Initialized;
        }

        public static IEnumerable<State> GetAll(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return GetAll();
            }

            return GetAll().Where(e => e.Name.Contains(name));
        }
    }
}
