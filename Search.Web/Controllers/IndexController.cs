﻿using Microsoft.AspNetCore.Mvc;
using Search.Core.Extensions;
using Search.IndexService;
using Search.IndexService.Models.Converters;
using System;
using System.Linq;

namespace Search.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class IndexController : ControllerBase
    {
        public IndexController(QueueForIndex queueForIndex)
        {
            _queueForIndex = queueForIndex;
        }
        
        [HttpPost]
        public IActionResult Index([FromQuery] Uri url)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (url == null)
                return BadRequest("Не задан URL.");
            if (!url.IsAbsoluteUri)
                return BadRequest($"\"{url}\" не является абсолютным URL.");

            var result = _queueForIndex.AddToQueueElement(url);
            return result.IsSuccess
                ? Ok()
                : StatusCode(result.Error.ToInt());
        }

        [HttpGet]
        public IActionResult GetQueue()
        {
            var requests = _queueForIndex.GetAllElementsQueue();
            if (requests.IsFailure)
                return StatusCode(requests.Error.ToInt());

            return Ok(
                requests.Value.Select(x => x.ToDbo())
            );
        }

        private readonly QueueForIndex _queueForIndex;
    }
}