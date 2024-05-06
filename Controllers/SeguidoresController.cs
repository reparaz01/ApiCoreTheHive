using ApiCoreTheHive.Models;
using ApiCoreTheHive.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiCoreTheHive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeguidoresController : ControllerBase
    {

        private RepositoryTheHive repo;

        public SeguidoresController(RepositoryTheHive repo)
        {
            this.repo = repo;
        }


        [Authorize]
        [HttpGet]
        [Route("[action]/{username}")]
        public ActionResult<List<Usuario>> GetSeguidores(string username)
        {
            return this.repo.GetSeguidores(username);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]/{username}")]
        public ActionResult<List<Usuario>> GetSeguidos(string username)
        {
            return this.repo.GetSeguidos(username);
        }


        [Authorize]
        [HttpGet]
        [Route("[action]/{username}")]
        public ActionResult<int> GetSeguidoresCount(string username)
        {
            return this.repo.GetSeguidoresCount(username);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]/{username}")]
        public ActionResult<int> GetSeguidosCount(string username)
        {
            return this.repo.GetSeguidosCount(username);
        }


        /* 
         * El método IsFollowing toma un parámetro username, 
         * que representa el nombre de usuario del usuario que se desea verificar si está siendo seguido
         * por el usuario autenticado actualmente
         * 
         */

        [Authorize]
        [HttpGet]
        [Route("[action]/{username}")]
        public ActionResult<bool> IsFollowing(string username)
        {
            string jsonUsuario = HttpContext.User.FindFirst(x => x.Type == "UserData").Value;
            Usuario usuarioLogeado = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            return this.repo.IsFollowing(usuarioLogeado.Username, username);
        }

        [Authorize]
        [HttpPost]
        [Route("[action]/{username}")]
        public async Task<IActionResult> Follow(string username)
        {
            string jsonUsuario = HttpContext.User.FindFirst(x => x.Type == "UserData").Value;
            Usuario usuarioLogeado = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            this.repo.Follow(usuarioLogeado.Username, username);
            return Ok();
        }

        [Authorize]
        [HttpDelete]
        [Route("[action]/{username}")]
        public async Task<IActionResult> Unfollow(string username)
        {
            string jsonUsuario = HttpContext.User.FindFirst(x => x.Type == "UserData").Value;
            Usuario usuarioLogeado = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            this.repo.Unfollow(usuarioLogeado.Username, username);
            return Ok();
        }








    }
}
