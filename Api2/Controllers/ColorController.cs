using Api2.Models;
using Api2;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Api.Controllers {
    [Route("color")]
    public class ColorController : ControllerBase {

        public ColorController() {
            Debug.WriteLine("ColorController c_tor called");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Delete/Role")]
        public IActionResult DeleteRole() {
            return new JsonResult(new ColorModel { Action = "Delete/Role", Red = 1, Green = 1, Blue = 1 });
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("Edit/Role")]
        public IActionResult EditRole() {
            return new JsonResult(new ColorModel { Action = "Edit/Role", Red = 2, Green = 2, Blue = 2 });
        }

        [Authorize(Roles = "Admin,User,Readonly")]
        [HttpGet("Get/Role")]
        public IActionResult GetRole() {
            return new JsonResult(new ColorModel { Action = "Get/Role", Red = 3, Green = 3, Blue = 3 });
        }


        [Authorize(Policy = "DeletePolicy")]
        [HttpGet("Delete/Policy")]
        public IActionResult DeletePolicy() {
            return new JsonResult(new ColorModel { Action = "Delete/Policy", Red = 4, Green = 4, Blue = 4 });
        }

        [Authorize(Policy = "EditPolicy")]
        [HttpGet("Edit/Policy")]
        public IActionResult EditPolicy() {
            return new JsonResult(new ColorModel { Action = "Edit/Policy", Red = 5, Green = 5, Blue = 5 });
        }

        [Authorize(Policy = "GetPolicy")]
        [HttpGet("Get/Policy")]
        public IActionResult GetPolicy() {
            return new JsonResult(new ColorModel { Action = "Get/Policy", Red = 6, Green = 6, Blue = 6 });
        }


    }
}
