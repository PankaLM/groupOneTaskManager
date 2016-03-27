using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Aggregates.Tasks
{
    public class Action
    {
        public static Action Trash = new Action(1, "Trash", "Изхвърли");

        public static Action Transfer = new Action(2, "Transfer", "Прехвърли на друг");

        public static Action Schedule = new Action(3, "Schedule", "Пренасрочи");

        public static Action Defer = new Action(4, "Defer", "Отсрочи");

        public static Action Delegate = new Action(5, "Delegate", "Делегирай");

        public static Action FollowUp = new Action(6, "Follow up", "Следи");

        public static Action Clarify = new Action(7, "Clarify", "Изясни");

        public static Action Simplify = new Action(8, "Simplify", "Опрости");

        public static Action Execute = new Action(9, "Execute", "Изпълни");

        private Action(int actionId, string nameEn, string name)
        {
            this.ActionId = actionId;
            this.Name = name;
            this.NameEn = nameEn;
        }

        public int ActionId { get; private set; }

        public string Name { get; private set; }

        public string NameEn { get; private set; }

        public static Action GetById(int id)
        {
            return GetAll().Where(e => e.ActionId == id).SingleOrDefault();
        }

        public static IEnumerable<Action> GetAll()
        {
            yield return Trash;
            yield return Transfer;
            yield return Schedule;
            yield return Defer;
            yield return Delegate;
            yield return FollowUp;
            yield return Clarify;
            yield return Simplify;
            yield return Execute;
        }

        public static IEnumerable<Action> GetAll(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return GetAll();
            }

            return GetAll().Where(e => e.Name.Contains(name));
        }
    }
}
