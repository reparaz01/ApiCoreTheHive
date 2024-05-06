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
    public class PublicacionesController : ControllerBase
    {

        private RepositoryTheHive repo;

        public PublicacionesController(RepositoryTheHive repo)
        {
            this.repo = repo;
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public ActionResult<List<Publicacion>> GetPublicaciones()
        {
            return this.repo.GetAllPublicaciones();
        }

        [Authorize]
        [HttpGet]
        [Route("[action]/{username}")]
        public ActionResult<List<Publicacion>> GetPublicacionesUsuario(string username)
        {
            return this.repo.GetPublicacionesUsuario(username);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]/{username}")]
        public ActionResult<List<Publicacion>> GetPublicacionesExceptoUsuario(string username)
        {
            return this.repo.GetAllPublicacionesExceptoUsuario(username);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]/{username}")]
        public ActionResult<List<Publicacion>> GetPublicacionesSeguidos(string username)
        {
            return this.repo.GetAllPublicacionesSeguidosExceptoUsuario(username);
        }


        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public ActionResult<int> GetNextPublicacionId()
        {
            return this.repo.GetNextPublicacionId();
        }


        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddPublicacion(Publicacion publicacion)
        {
            await this.repo.AddPublicacion(publicacion);
            return Ok();
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete]
        [Route("[action]/{idpublicacion}")]
        public ActionResult DeletePublicacion(int idpublicacion)
        {
            string jsonUsuario = HttpContext.User.FindFirst(x => x.Type == "UserData").Value;
            Usuario usuarioLogeado = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);

            this.repo.EliminarPublicacion(idpublicacion, usuarioLogeado.Username);
            return Ok();
        }




    }
}
