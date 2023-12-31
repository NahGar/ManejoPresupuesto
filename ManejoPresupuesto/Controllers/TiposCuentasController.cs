﻿using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Controllers {
    public class TiposCuentasController: Controller {

        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;

        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas,
                                      IServicioUsuarios servicioUsuarios) {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
        }

        //se utiliza Index para la vista que va a mostrar un listado de elementos
        public async Task<IActionResult> Index() {

            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            return View(tiposCuentas);
        }

        //Este es el GET
        public IActionResult Crear() {

            return View();
        }

        //respuesta al post de la vista Crear
        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta) {

            //Esto es para verificar que se cumple por ejemplo el [Required] de Nombre
            if(!ModelState.IsValid) {
                return View(tipoCuenta);
            }

            tipoCuenta.UsuarioId = servicioUsuarios.ObtenerUsuarioId();
            
            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(tipoCuenta.Nombre,tipoCuenta.UsuarioId);
            if(yaExisteTipoCuenta) {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya existe.");
                return View(tipoCuenta);
            }


            await repositorioTiposCuentas.Crear(tipoCuenta);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int id) {

            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if(tipoCuenta == null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(TipoCuenta tipoCuenta) {

            //Esto es para verificar que se cumple por ejemplo el [Required] de Nombre
            if (!ModelState.IsValid) {
                return View(tipoCuenta);
            }

            int usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);

            if(tipoCuentaExiste is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTiposCuentas.Actualizar(tipoCuenta);
            return RedirectToAction("Index");
            
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id) {

            int usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if (tipoCuentaExiste is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuentaExiste);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id) {

            int usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if (tipoCuentaExiste is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTiposCuentas.Borrar(id);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre, int id) {
            
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(nombre, usuarioId, id);

            if(yaExisteTipoCuenta) {
                return Json($"El nombre {nombre} ya existe");
            }

            return Json(true);
        }

        //[FromBody] = del cuerpo de la petición HTTP
        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids) {

            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            //validar que los tiposCuentas son del usuario
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            //LINQ
            var idsTiposCuentas = tiposCuentas.Select(x => x.Id);

            //Verifica si hay algun id que no le pertenezca al usuario
            var idsTiposCuentasNoPertenecenAlUsuario = ids.Except(idsTiposCuentas).ToList();

            if(idsTiposCuentasNoPertenecenAlUsuario.Count > 0) {
                //prohibir
                return Forbid();
            }

            //LINQ
            var tiposCuentasOrdenados = ids.Select((valor, indice) =>
                new TipoCuenta() { Id = valor, Orden = indice + 1 }).AsEnumerable();

            await repositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);

            return Ok();
        }
    }
}
