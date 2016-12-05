using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;

/* se importa la la referencia al proyecto desde el menu proyecto y luego agregar referencia, hecho esto
 * se procede a importar las librerias de la referencia en este caso se importan dos librerias. */
using Oracle.DataAccess.Client;
using System.Data; //en algunos casos esta libreria se agrega automáticamente, solo se agrega cuando no está;

namespace RegionesOracle
{
    public partial class Regiones : Form
    {
        /* Declaramos las variables necesarias para hacer la conexión, como estan directamente dentro de la clase
         * "Regiones" a estas variables se les conoce como Campos de la clase o atributos de la clase, y deven ser
         * privadas para esto se emplea el uso del modificador de acceso private, a ser campos de la clase solo
         * es necesario declararlas una ves para así poder usar las mismas variables en cualquier Botón de la forma. */
        OracleConnection conexion;
        OracleCommand comando;
        OracleDataReader receptorConsulta;

        string nombreHost = Interaction.InputBox("Ingrese el nombre del Host");
        string contrasenia = Interaction.InputBox("Ingrese la contraseña de la base de datos");
        string consulta;
        string cadenaConexion;

        public Regiones()
        {            
            InitializeComponent();

            //crear un nuevo objeto de conexion del tipo OracleConnection
            conexion = new OracleConnection();

            /* crear el string de conexion usando la variable cadenaConexion y colocando los parametros respectivos
            * de la máquina de la cual obtendremos la base de datos */
            cadenaConexion = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + nombreHost + ")(PORT = 1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=XE)));User Id=HR; password=" + contrasenia + ";";

            //inicio de un bloque try para prevenir desbordaciones
            try
            {
                //pasar la cadena de conexión a la conexion en su propiedad de conexión
                conexion.ConnectionString = cadenaConexion;
                //abrir la conexió
                conexion.Open();                
            }
            catch (Exception error)
            {
                //liberar los recursos de la conexión si sucede un error y luego mostrar el error correspondiente
                conexion.Dispose();
                MessageBox.Show("No se ha podido conectar" + error.Message);
            }
        }

//La conexión debe estar abierta para esta operación
        public void btnBuscar_Click(object sender, EventArgs e)
        {
            //inicio de try para prevenir desbordes de la aplicacion por errores en la consulta
            try
            {
                //realizar consulta en base al id de la región que ingrese el usuario
                consulta = "SELECT "
                         + "REGION_NAME "
                         + "FROM "
                         + "REGIONS "
                         + "WHERE "
                         + "REGION_ID = " + txtIdRegion.Text;
                
                //crear comando como un nuevo objeto del tipo OracleCommand enviando los argumentos consulta y conexión
                comando = new OracleCommand(consulta, conexion);
                //indicar que el comando es del tipo Texto
                comando.CommandType = CommandType.Text;
                //inicializar el receptor de la consulta ejecutando el comando en su propiedad ExecuteReader
                receptorConsulta = comando.ExecuteReader();
                //iniciar la lectura de datos del receptorConsuta
                receptorConsulta.Read();
            }
            catch (Exception error)
            {
                //inicar al usuario que ocurrio un error durante la consulta
                MessageBox.Show("Ocurrio un error al consultar, la aplicación se cerrará. Detalles: " + error.Message + "\n " + consulta);
                //cerrar la aplicación despues de notificar el error
                Environment.Exit(0); //el 0 indica que la aplicacion finalizará correctamente debido a un error
            }

            //usar if para verificar que la consulta arroje resultados
            if (receptorConsulta.HasRows == true)
            {
                //si la consulta tiene resultado, entonces mostrar el resultado en el TextBox corresponiente
                txtNombreRegion.Text = receptorConsulta["REGION_NAME"].ToString(); //recuperar nombre de la región en base a su columna, en este caso "REGION_NAME" y luego convertirlo a String para poder mostrarlo en el TextBox correspondiente
                //colocar el foco en txtIdRegion
                txtIdRegion.Focus();
                //seleccionar el texto que contenga txtIdRegion
                txtIdRegion.SelectAll();
            }
            else
            {
                //si la consulta no tiene resultados, se notifica al usuario con un mensaje
                MessageBox.Show("No se encontraron resultados");
                //limpiar TextBox
                txtIdRegion.Clear();
                txtNombreRegion.Clear();
                //colocar el foco en txtIdRegion
                txtIdRegion.Focus();
            }

            //liberar los recursos al final de la ejecución del código en el botón
            receptorConsulta.Dispose();
            comando.Dispose();            
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            //inicio de try para prevenir desbordes de la aplicacion por errores en la consulta
            try
            {
                //realizar consulta en base al id de la región que ingrese el usuario
                consulta = "UPDATE REGIONS "
                         + "SET "
                         + "REGION_NAME = " + "'" + txtNombreRegion.Text + "' "
                         + " WHERE "
                         + " REGION_ID = " + txtIdRegion.Text;

                //crear comando como un nuevo objeto del tipo OracleCommand enviando los argumentos consulta y conexión
                comando = new OracleCommand(consulta, conexion);
                //indicar que el comando es del tipo Texto
                comando.CommandType = CommandType.Text;
                //indicar que la consulta se ejecuta pero no trae resultados
                comando.ExecuteNonQuery();

             }

            catch (Exception error)
            {
                //inicar al usuario que ocurrio un error durante la consulta
                MessageBox.Show("Ocurrio un error al consultar, la aplicación se cerrará. Detalles: " + error.Message + "\n " + consulta);
                //cerrar la aplicación despues de notificar el error
                Environment.Exit(0); //el 0 indica que la aplicacion finalizará correctamente debido a un error
            }
            //notificar al usuario que se actualizó correctamente el registro
            MessageBox.Show("Datos actualizados correctamente");


            //colocar el foco en txtIdRegion
            txtIdRegion.Focus();
            //seleccionar el texto que contenga txtIdRegion
            txtIdRegion.SelectAll();

            //liberar los recursos al final de la ejecución del código en el botón
            receptorConsulta.Dispose();
            comando.Dispose();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            //inicio de try para prevenir desbordes de la aplicacion por errores en la consulta
            try
            {
                //realizar consulta en base al id de la región que ingrese el usuario
                consulta = "INSERT INTO REGIONS "
                         + "(REGION_ID, "
                         + "REGION_NAME) "
                         + "VALUES "
                         + "(" + txtIdRegion.Text + ", "
                         + "'" + txtNombreRegion.Text + "')";

                //crear comando como un nuevo objeto del tipo OracleCommand enviando los argumentos consulta y conexión
                comando = new OracleCommand(consulta, conexion);
                //indicar que el comando es del tipo Texto
                comando.CommandType = CommandType.Text;
                //indicar que la consulta se ejecuta pero no trae resultados
                comando.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                //inicar al usuario que ocurrio un error durante la consulta
                MessageBox.Show("Ocurrio un error al consultar, la aplicación se cerrará. Detalles: " + error.Message + "\n " + consulta);
                //cerrar la aplicación despues de notificar el error
                //inicio del bloque try para controlar el cierre de la aplicación
                Environment.Exit(0); //el 0 indica que la aplicacion finalizará correctamente debido a un error
            }

            //notificar al usuario que se agregó correctamente el registro
            MessageBox.Show("Datos agregados correctamente");
            //colocar el foco en txtIdRegion
            txtIdRegion.Focus();
            //seleccionar el texto que contenga txtIdRegion
            txtIdRegion.SelectAll();

            //liber los recursos al final de la ejecución del código en el botón
            receptorConsulta.Dispose();
            comando.Dispose();            
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            //inicio de try para prevenir desbordes de la aplicacion por errores en la consulta
            try
            {
                //realizar consulta en base al id de la región que ingrese el usuario
                consulta = "DELETE FROM REGIONS "
                         + "WHERE "
                         + "REGION_ID = " + txtIdRegion.Text;

                //crear comando como un nuevo objeto del tipo OracleCommand enviando los argumentos consulta y conexión
                comando = new OracleCommand(consulta, conexion);
                //indicar que el comando es del tipo Texto
                comando.CommandType = CommandType.Text;
                //indicar que la consulta se ejecuta pero no trae resultados
                comando.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                //inicar al usuario que ocurrio un error durante la consulta
                MessageBox.Show("Ocurrio un error al consultar, la aplicación se cerrará. Detalles: " + error.Message + "\n " + consulta);
                //cerrar la aplicación despues de notificar el error
                //inicio del bloque try para controlar el cierre de la aplicación
                Environment.Exit(0); //el 0 indica que la aplicacion finalizará correctamente debido a un error
            }

            //notificar al usuario que se agregó correctamente el registro
            MessageBox.Show("Datos eliminados correctamente");
            //limpiar TextBox
            txtIdRegion.Clear();
            txtNombreRegion.Clear();
            //colocar el foco en txtIdRegion
            txtIdRegion.Focus();

            //liber los recursos al final de la ejecución del código en el botón
            receptorConsulta.Dispose();
            comando.Dispose();
        }

        private void btnMostrarLista_Click(object sender, EventArgs e)
        {
            //inicio de try para prevenir desbordes de la aplicacion por errores en la consulta
            try
            {
                //realizar consulta de todos los registros
                consulta = "SELECT "
                         + "REGION_ID, "
                         + "REGION_NAME "
                         + "FROM "
                         + "REGIONS ";


                //crear comando como un nuevo objeto del tipo OracleCommand enviando los argumentos consulta y conexión
                comando = new OracleCommand(consulta, conexion);
                //indicar que el comando es del tipo Texto
                comando.CommandType = CommandType.Text;
                //iniciarlizar el receptor de la consulta ejecutando el comando en su propiedad ExecuteReader
                receptorConsulta = comando.ExecuteReader();
            }
            catch (Exception error)
            {
                //inicar al usuario que ocurrio un error durante la consulta
                MessageBox.Show("Ocurrio un error al consultar, la aplicación se cerrará. Detalles: " + error.Message + "\n " + consulta);
                //cerrar la aplicación despues de notificar el error
                Environment.Exit(0); //el 0 indica que la aplicacion finalizará correctamente debido a un error
            }

            //usar un ciclo whila para llenar el listBox con todos los registros mientras receptorConsulta esta leyendo
            //los resultados de la consulta
            while (receptorConsulta.Read()) //iniciar la lectura de datos del receptorConsuta
            {
                //agregar cada registro recuperado en una linea del listBox;

                //recuper en cada pasada el id de la region y el nombre de la region
                contenedorLista.Items.Add("La región " + receptorConsulta["REGION_ID"].ToString() + " es: " + receptorConsulta["REGION_NAME"].ToString());
            }

            //liber los recursos al final de la ejecución del código en el botón
            receptorConsulta.Dispose();
            comando.Dispose();
         }
    }
}

