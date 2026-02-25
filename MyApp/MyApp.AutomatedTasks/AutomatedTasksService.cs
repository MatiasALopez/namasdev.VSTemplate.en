using System;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using namasdev.Core.Exceptions;
using namasdev.Core.Validation;

using MyApp.Business;

namespace MyApp.AutomatedTasks
{
    public class AutomatedTasksService : IHostedService
    {
        // NOTA (ML): definir dependencias
        //private readonly IRepositorioA _repositorioA;
        //private readonly INegocioA _negocioA;

        private Task _backgroundTask;
        private CancellationTokenSource _cancellationTokenSource;
        private TimeSpan _tiempoEsperaEjecucion;

        // NOTA (ML): inyectar dependencias
        //public TareasAutomaticasService(IRepositorioA repositorioA, INegocioA negocioA)
        //{
            //Validador.ValidarArgumentRequeridoYThrow(repositorioA, nameof(repositorioA));
            //Validador.ValidarArgumentRequeridoYThrow(negocioA, nameof(negocioA));

            //_repositorioA = repositorioA;
            //_negocioA = negocioA;
        //}

        public AutomatedTasksService()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Iniciando tareas automaticas...");

            CultureInfo.DefaultThreadCurrentCulture =
            CultureInfo.DefaultThreadCurrentUICulture =
                new CultureInfo(ConfigurationManager.AppSettings["Culture"]);

            _tiempoEsperaEjecucion = TimeSpan.FromMinutes(int.Parse(ConfigurationManager.AppSettings["ExecutionDelayInMin"]));

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _backgroundTask = Task.Run(() => EjecutarTareasAsync(_cancellationTokenSource.Token));

            Console.WriteLine($"Tareas automaticas iniciadas.");

            return Task.CompletedTask;
        }

        private async Task EjecutarTareasAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Task.WaitAll(ObtenerTareasAEjecutar());

                    await Task.Delay(_tiempoEsperaEjecucion, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            Console.WriteLine("Tareas automaticas finalizadas.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Tareas automaticas detenidas.");

            _cancellationTokenSource.Cancel();

            return Task.WhenAny(_backgroundTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        private Task[] ObtenerTareasAEjecutar()
        {
            return new Task[]
            {
                //EjecutarTarea1Async(),
            };
        }

        //async Task EjecutarTarea1Async()
        //{
        //    try
        //    {
        //        // logica tarea 1
        //    }
        //    catch (Exception ex)
        //    {
        //        _erroresNegocio.AgregarYObtenerMensajeAlUsuario(ex);

        //        Console.WriteLine($"[ERROR] No se pudo ejecutar la tarea 1. " + ExceptionHelper.ObtenerMensajesRecursivamente(ex));
        //    }
        //}
    }
}
