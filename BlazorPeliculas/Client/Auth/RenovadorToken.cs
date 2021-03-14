using System;
using System.Timers;

namespace BlazorPeliculas.Client.Auth
{
    public class RenovadorToken : IDisposable
    {

        private readonly ILoginService _loginService;

        public RenovadorToken(ILoginService loginService)
        {
            _loginService = loginService;
        }

        Timer timer;


        public void Iniciar()
        {
            timer = new Timer();
            timer.Interval = 5000; // 4 minutos
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _loginService.ManejadorRenovacionToken();
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
