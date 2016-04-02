using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TaskManager.Domain.Aggregates.Tasks;
using TaskManager.Domain.Data.ViewObjects;

namespace TaskManager.Web.Api.Tasks.Controllers
{
    [RoutePrefix("api/noms/states")]

    public class StateNomsController : ApiController
    {
        [Route("")]
        [HttpGet]
        public IEnumerable<ValueVo> GetStates(string term = null)
        {
            return State.GetAll(term).Select(s =>
                new ValueVo(s.StateId, s.Name));
        }

        [Route("{id:int}")]
        [HttpGet]
        public ValueVo GetState(int id)
        {
            var state = State.GetById(id);

            return new ValueVo(state.StateId, state.Name);
        }
    }
}
