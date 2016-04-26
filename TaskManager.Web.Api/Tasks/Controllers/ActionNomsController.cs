using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TaskManager.Domain.Data.ViewObjects;

namespace TaskManager.Web.Api.Tasks.Controllers
{
    [RoutePrefix("api/noms/actions")]

    public class ActionNomsController : ApiController
    {
        [Route("")]
        [HttpGet]
        public IEnumerable<ValueVo> GetActions(string term = null)
        {
            return Domain.Aggregates.Tasks.Action.GetAll(term).Select(s =>
                new ValueVo(s.ActionId, s.Name));
        }

        [Route("{id:int}")]
        [HttpGet]
        public ValueVo GetAction(int id)
        {
            var state = Domain.Aggregates.Tasks.Action.GetById(id);

            return new ValueVo(state.ActionId, state.Name);
        }
    }
}
