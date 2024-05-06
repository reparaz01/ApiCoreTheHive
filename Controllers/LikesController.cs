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
    public class LikesController : ControllerBase
    {

        private RepositoryTheHive repo;

        public LikesController(RepositoryTheHive repo)
        {
            this.repo = repo;
        }


        [Authorize]
        [HttpGet]
        [Route("[action]/{idpublicacion}")]
        public ActionResult<int> GetLikesPublicacion(int idpublicacion)
        {
            return this.repo.GetLikes(idpublicacion);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]/{idpublicacion}/{username}")]
        public ActionResult<bool> IsLiked(int idpublicacion, string username)
        {
            return this.repo.IsLiked(idpublicacion, username);
        }

 
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        [Route("[action]/{idpublicacion}")]
        public ActionResult Like(int idpublicacion)
        {

            string jsonUsuario = HttpContext.User.FindFirst(x => x.Type == "UserData").Value;
            Usuario usuarioLogeado = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);

            this.repo.Like(idpublicacion, usuarioLogeado.Username);
            return Ok();
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete]
        [Route("[action]/{idpublicacion}")]
        public ActionResult Dislike(int idpublicacion)
        {
            string jsonUsuario = HttpContext.User.FindFirst(x => x.Type == "UserData").Value;
            Usuario usuarioLogeado = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);

            this.repo.Dislike(idpublicacion, usuarioLogeado.Username);
            return Ok();
        }


    }
}
