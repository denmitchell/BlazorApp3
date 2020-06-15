using Api1.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Api.Controllers {
    [Route("identity")]
    public class IdentityController : ControllerBase {

        public IdentityController() {
            Debug.WriteLine("IdentityController c_tor called");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Delete/Role")]
        public IActionResult DeleteRole() {
            return new JsonResult(new ActionModel { Action = "Delete/Role", StatusCodeText = "Success" });
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("Edit/Role")]
        public IActionResult EditRole() {
            return new JsonResult(new ActionModel { Action = "Edit/Role", StatusCodeText = "Success" });
        }

        [Authorize(Roles = "Admin,User,Readonly")]
        [HttpGet("Get/Role")]
        public IActionResult GetRole() {
            return new JsonResult(new ActionModel { Action = "Get/Role", StatusCodeText = "Success" });
        }

        [HttpGet("Get/Role/Open")]
        public IActionResult GetRole2() {
            return new JsonResult(new ActionModel { Action = "Get/Role/Open", StatusCodeText = "Success" });
        }


        [Authorize(Policy = "DeletePolicy")]
        [HttpGet("Delete/Policy")]
        public IActionResult DeletePolicy() {
            return new JsonResult(new ActionModel { Action = "Delete/Policy", StatusCodeText = "Success" });
        }

        [Authorize(Policy = "EditPolicy")]
        [HttpGet("Edit/Policy")]
        public IActionResult EditPolicy() {
            return new JsonResult(new ActionModel { Action = "Edit/Policy", StatusCodeText = "Success" });
        }

        [Authorize(Policy = "GetPolicy")]
        [HttpGet("Get/Policy")]
        public IActionResult GetPolicy() {
            return new JsonResult(new ActionModel { Action = "Get/Policy", StatusCodeText = "Success" });
        }


    }
}
