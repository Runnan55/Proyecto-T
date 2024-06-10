using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public interface INivel
    {
        void Iniciar();
        void Finalizar();
        void SpawnEnemigos();
        void AbrirPuertas();
        void CerrarPuertas();
        void EnemigoDerrotado(Enemy enemigo);
    }

    public class NivelArena : INivel
    {
        private List<Enemy> enemigos = new List<Enemy>();
        private GameObject puertaEntrada, puertaSalida;

        public NivelArena(GameObject entrada, GameObject salida)
        {
            this.puertaEntrada = entrada;
            this.puertaSalida = salida;
        }

        public void Iniciar()
        {
            CerrarPuertas();
            SpawnEnemigos();
        }

        public void Finalizar()
        {
            AbrirPuertas();
        }

        public void SpawnEnemigos()
        {

        }

        public void AbrirPuertas()
        {

        }

        public void CerrarPuertas()
        {

        }

        public void EnemigoDerrotado(Enemy enemigo)
        {
            VerificarFinalizacion();
        }

        private void VerificarFinalizacion()
        {
            if (enemigos.Count == 0)
            {
                Finalizar();
            }
        }
    }
}
