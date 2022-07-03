using Microsoft.AspNetCore.Mvc;
using WorkingWithRabbitMq.Application;
using WorkingWithRabbitMq.Application.Model;
using WorkingWithRabbitMq.Infra.Interface;

namespace ProjectWorkingWithRabbitMq.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkingWithRabbitMqController : ControllerBase
    {        
        private readonly IWorkingWithRabbitMqService _workingWithRabbitMqService;        

        public WorkingWithRabbitMqController(IWorkingWithRabbitMqService workingWithRabbitMqService)
        {            
            _workingWithRabbitMqService = workingWithRabbitMqService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Set(RabbitMqTask task)
        {            
            if (task.IsValid())
                return BadRequest();

            _workingWithRabbitMqService.SendMessage(task);

            return Created("", "");
        }

        [HttpGet]
        [ProducesResponseType(typeof(RabbitMqTask), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get()
        {
            var response = _workingWithRabbitMqService.GetMessage();

            if (response == null)
                return NotFound();

            return Ok(response);
        }
    }
}