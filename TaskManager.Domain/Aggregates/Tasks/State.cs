using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Aggregates.Tasks
{
    public class State
    {
        public static State Deleted = new State(1, "Deleted", "Изтрита");

        public static State InProgress = new State(2, "InProgress", "В прогрес");

        public static State Done = new State(3, "Done", "Готова");

        public static State Initialized = new State(4, "Initialized", "Инициализирана");

        private State(int StateId, string nameEn, string name)
        {
            this.StateId = StateId;
            this.Name = name;
            this.NameEn = nameEn;
        }

        public int StateId { get; private set; }

        public string Name { get; private set; }

        public string NameEn { get; private set; }

        public static State GetById(int id)
        {
            return GetAll().Where(e => e.StateId == id).SingleOrDefault();
        }

        public static IEnumerable<State> GetAll()
        {
            yield return Deleted;
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
