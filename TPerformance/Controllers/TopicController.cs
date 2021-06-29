using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TPerformance.Models;

namespace TPerformance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly TPContext _dbContext;
        private readonly IWebHostEnvironment _enviroment;

        public TopicController(TPContext dbContext, IWebHostEnvironment enviroment)
        {
            _dbContext = dbContext;
            _enviroment = enviroment;
        }
        [Authorize]
        [HttpGet("GetTopics")]
        public List<Topic> GetTopics(int take = 1)
        {
            // check if he has access to catch this data
           return  _dbContext.topics
                .Skip((take - 1) * 10)
                .Take(10)
                .ToList();
        }
        [Authorize]
        [HttpGet("GetTopic")]
        public IActionResult GetTopicById(int Id)
        {
            // check if he has access to catch this data
            var topic = _dbContext.topics.Find(Id);

            return Ok(topic);
        }
        [Authorize]
        [HttpGet("GetMyOwnTopics/{CustomerId}/{take}")]
        public List<Topic> GetTopicsByCustomerId(int CustomerId , int take = 1)
        {
            // check if he has access to catch this data

            //check if customer id empty
            if (CustomerId <= 0)
            {
                NotFound("CustomerId Is not Correct");
            }
            return _dbContext.topics
                 .Where(customer =>customer.CustomerId == CustomerId)
                 .Skip((take - 1) * 10)
                 .Take(10)
                 .ToList();
        }
        [Authorize ]
        [HttpPost("addTopic")]
        public IActionResult InsertTopic([FromBody]Topic topic,[FromForm]IFormCollection file)
        {
            // check if he has access to catch this data
            if (string.IsNullOrEmpty(topic.Title) || string.IsNullOrEmpty(topic.Description))
            {
                return StatusCode(500, "Please fill the topic attribute");
            }
            if (file == null)
            {
                return StatusCode(500, "Please add a picture!");
            }
            // to check if this order is from the mbile application or manual from system admin
            try
            {

                        if (!Directory.Exists(_enviroment.WebRootPath + "\\Upload\\"))
                        {
                            Directory.CreateDirectory(_enviroment.WebRootPath + "\\Upload\\");
                        }
                        using (FileStream fileStream = System.IO.File.Create(_enviroment.WebRootPath + "\\Upload\\" + Guid.NewGuid().ToString() + file.Files[0].FileName))
                        {
                            file.Files[0].CopyTo(fileStream);
                            fileStream.Flush();
                            topic.ImageUrl = fileStream.Name.ToString();
                            _dbContext.topics.Add(topic);
                        }

                    _dbContext.SaveChanges();
                
            }
            catch (Exception e)
            {
                return StatusCode(500, "Could No save the Picture");
            }

            return StatusCode(201);
        }
        // gettting data from form application 
        [Authorize]
        [HttpPost("NewTopic")]
        public IActionResult InsertTopic([FromForm] string title , [FromForm]string description,  IFormFile file)
        {
            // check if he has access to catch this data
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description))
            {
                return StatusCode(500, "Please fill the topic attribute");
            }
            if (file == null)
            {
                return StatusCode(500, "Please add a picture!");
            }
            Topic topic = new Topic
            {
                Title = title,
                Description = description
            };
            // to check if this order is from the mbile application or manual from system admin
            try
            {

                if (!Directory.Exists(_enviroment.WebRootPath + "\\Upload\\"))
                {
                    Directory.CreateDirectory(_enviroment.WebRootPath + "\\Upload\\");
                }
                using (FileStream fileStream = System.IO.File.Create(_enviroment.WebRootPath + "\\Upload\\" + Guid.NewGuid().ToString() + file.FileName))
                {
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                    topic.ImageUrl = fileStream.Name.ToString();
                    _dbContext.topics.Add(topic);
                }

                _dbContext.SaveChanges();

            }
            catch (Exception e)
            {
                return StatusCode(500, "Could No save the Picture");
            }

            return StatusCode(201);
        }

        [Authorize(Roles ="Admin")]
        [HttpPut("updateTopic")]
        public IActionResult UpdateTopic(int id, [FromBody] Topic topic, IFormFile file)
        {
            if (id != topic.Id)
            {
                return StatusCode(400, "id is not correct");
            }
            // check if he has access to catch this data
            if (string.IsNullOrEmpty(topic.Title) || string.IsNullOrEmpty(topic.Description))
            {
                return StatusCode(500, "Please fill the topic attribute");
            }
            var topicObject  =  _dbContext.topics.Find(id);

            if (topicObject == null)
            {
                return NotFound("object is not found");
            }
            // to check if this order is from the mbile application or manual from system admin
            try
            {
                if (file == null)
                {

                    if (!Directory.Exists(_enviroment.WebRootPath + "\\Upload\\"))
                    {
                        Directory.CreateDirectory(_enviroment.WebRootPath + "\\Upload\\");
                    }
                    using (FileStream fileStream = System.IO.File.Create(_enviroment.WebRootPath + "\\Upload\\" + Guid.NewGuid().ToString() + file.FileName))
                    {
                        file.CopyTo(fileStream);
                        fileStream.Flush();
                        topic.ImageUrl = fileStream.Name.ToString();
                    }

                }
  
                _dbContext.Entry(topicObject).State = EntityState.Modified;

                _dbContext.SaveChanges();

            }
            catch (Exception e)
            {
                return StatusCode(500, "Could No save the Picture");
            }

            return StatusCode(201);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("editTopic")]
        public IActionResult editTopic(int id,  Topic topic, [FromForm] IFormFile file)
        {
            if (id != topic.Id)
            {
                return StatusCode(400, "id is not correct");
            }
            // check if he has access to catch this data
            if (string.IsNullOrEmpty(topic.Title) || string.IsNullOrEmpty(topic.Description))
            {
                return StatusCode(500, "Please fill the topic attribute");
            }
            var topicObject = _dbContext.topics.Find(id);

            if (topicObject == null)
            {
                return NotFound("object is not found");
            }
            // to check if this order is from the mbile application or manual from system admin
            try
            {
                if (file == null)
                {

                    if (!Directory.Exists(_enviroment.WebRootPath + "\\Upload\\"))
                    {
                        Directory.CreateDirectory(_enviroment.WebRootPath + "\\Upload\\");
                    }
                    using (FileStream fileStream = System.IO.File.Create(_enviroment.WebRootPath + "\\Upload\\" + Guid.NewGuid().ToString() + file.FileName))
                    {
                        file.CopyTo(fileStream);
                        fileStream.Flush();
                        topic.ImageUrl = fileStream.Name.ToString();
                    }

                }

                _dbContext.Entry(topicObject).State = EntityState.Modified;

                _dbContext.SaveChanges();

            }
            catch (Exception e)
            {
                return StatusCode(500, "Could No save the Picture");
            }

            return StatusCode(201);
        }
        [Authorize(Roles ="Admin")]
        [HttpDelete("deleteTopic")]
        public IActionResult DeleteTopic(int id)
        {
            var topic = _dbContext.topics.Find(id);

            if (topic != null)
            {
                _dbContext.topics.Remove(topic);
            }
            // check if he has access to catch this data

            return Ok("deleted Successful");
        }
    }
}
