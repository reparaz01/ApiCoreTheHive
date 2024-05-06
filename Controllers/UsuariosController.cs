using ApiCoreTheHive.Helpers;
using ApiCoreTheHive.Models;
using ApiCoreTheHive.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using YamlDotNet.Core.Tokens;

namespace ApiCoreTheHive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {

        private RepositoryTheHive repo;

        public UsuariosController(RepositoryTheHive repo)
        {
            this.repo = repo;
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public ActionResult<List<Usuario>> GetUsuarios()
        {
            return this.repo.GetUsuarios();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("[action]/{username}")]
        public ActionResult <Usuario> FindUsuario(string username)
        {
            var usuario = this.repo.GetUser(username);
            if (usuario == null)
            {
                return NotFound();
            }
            return usuario;
        }

        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("[action]/{username}")]
        public ActionResult UsuarioExists(string username)
        {
            var found =  this.repo.UsuarioExists(username);
            if (found == false)
            {
                return NotFound();
            }
            return Ok();
        }

        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("[action]/{email}")]
        public ActionResult<bool> EmailExists(string email)
        {
            var found = this.repo.EmailExists(email);
            if (found == false)
            {
                return NotFound();
            }
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddUsuario(NuevoUsuario usuario)
        {
            if (repo.UsuarioExists(usuario.Username))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Ya existe un usuario con ese nombre de usuario.");
            }

            if (repo.EmailExists(usuario.Email))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Ya existe un usuario con ese email.");
            }

            await this.repo.RegisterUserAsync(usuario.Username, usuario.Password, usuario.Email, usuario.Nombre);
            return Ok();
        }


        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdateUsuario(Usuario usuario)
        {

            string jsonUsuario = HttpContext.User.FindFirst(x => x.Type == "UserData").Value;
            Usuario usuarioLogeado = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);

            if (usuarioLogeado.Username != usuario.Username)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Estas intentando modificar los datos de otro usuario.");
            }


            if (!repo.UsuarioExists(usuario.Username))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "No existe un usuario con ese nombre de usuario.");
            }

            await this.repo.UpdateUser(usuario);
            return Ok();
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpDelete]
        [Route("[action]/{username}")]
        public async Task<IActionResult> DeleteUsuario(string username)
        {

            string jsonUsuario = HttpContext.User.FindFirst(x => x.Type == "UserData").Value;
            Usuario usuarioLogeado = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);

            if (usuarioLogeado.Username != username)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Estas intentando eliminar un usuario que no es el tuyo.");
            }

            if (!repo.UsuarioExists(username))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "No existe un usuario con ese nombre de usuario.");
            }

            await this.repo.DeleteUser(username);
            return Ok();
        }




    }
}
