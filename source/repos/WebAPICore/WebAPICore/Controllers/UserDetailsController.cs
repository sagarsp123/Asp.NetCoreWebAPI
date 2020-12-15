using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPICore.Data;
using WebAPICore.Models;

namespace WebAPICore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class UserDetailsController : ControllerBase
    {
        private readonly EmployeeContext _context;

        public UserDetailsController(EmployeeContext context)
        {
            _context = context;
        }

        // GET: api/UserDetails
        [HttpGet]
        public IEnumerable<UserDetails> GetUserdetails()
        {
            return _context.Userdetails;
        }

        //[HttpGet]
        //public IEnumerable<UserDetails> GetUserdetails()
        //{
        //    return _context.Userdetails.FromSql<UserDetails>("spAddNewUser").ToList();
        //}

        // GET: api/UserDetails/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetUserDetails([FromRoute] long id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var userDetails = await _context.Userdetails.FindAsync(id);

        //    if (userDetails == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(userDetails);
        //}

        //[HttpGet("{EmailID}")]
        //public UserDetails GetUserDetails([FromRoute] string EmailID)
        //{
        //    var userDetail = _context.Userdetails.FromSql<UserDetails>("spCheckUserDetailsById {0}", EmailID).ToList().FirstOrDefault();
        //    //if(userDetail.EmailID == EmailID)
        //    //{
        //    //    userDetail.EmailID = "true";
        //    //}
        //    return userDetail;
        //}

        [HttpGet("{EmailID}")]
        public CurrentUserDetails GetUserDetails([FromRoute] string EmailID)
        {
            var userDetail = _context.CurrentUsers.FromSql<CurrentUserDetails>("spCheckUserDetailsById {0}", EmailID).ToList().FirstOrDefault();
            //if(userDetail.EmailID == EmailID)
            //{
            //    userDetail.EmailID = "true";
            //}
            return userDetail;
        }

        // PUT: api/UserDetails/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserDetails([FromRoute] long id, [FromBody] UserDetails userDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userDetails.UserID)
            {
                return BadRequest();
            }

            _context.Entry(userDetails).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserDetailsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserDetails
        [HttpPost]
        public async  Task<IActionResult> PostUserDetails([FromBody] UserDetails userDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Userdetails.Add(userDetails);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserDetails", new { id = userDetails.UserID }, userDetails);
        }

        // DELETE: api/UserDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserDetails([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDetails = await _context.Userdetails.FindAsync(id);
            if (userDetails == null)
            {
                return NotFound();
            }

            _context.Userdetails.Remove(userDetails);
            await _context.SaveChangesAsync();

            return Ok(userDetails);
        }

        private bool UserDetailsExists(long id)
        {
            return _context.Userdetails.Any(e => e.UserID == id);
        }
    }
}