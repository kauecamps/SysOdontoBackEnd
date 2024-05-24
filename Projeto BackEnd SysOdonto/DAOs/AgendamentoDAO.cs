﻿using MySql.Data.MySqlClient;
using Projeto_BackEnd_SysOdonto.DTOs;

namespace Projeto_BackEnd_SysOdonto.DAOs
{
    public class AgendamentoDAO
    {
        public void CadastrarAgendamento(CriarAgendamentoDTO agendamento, int idClinica)
        {
            var data = $"{agendamento.Ano}/{agendamento.Mes}/{agendamento.Dia}";

            var conexao = ConnectionFactory.Build();
            conexao.Open();

            var query = @"INSERT INTO Agendamento (Dentista, Paciente, DataDaConsulta, Horario, Observacao, Clinica) VALUES
						(@dentista,@paciente, @data, @horario, @observacoes, @clinica)";

            var comando = new MySqlCommand(query, conexao);
            comando.Parameters.AddWithValue("@dentista", agendamento.Dentista);
            comando.Parameters.AddWithValue("@paciente", agendamento.Paciente);
            comando.Parameters.AddWithValue("@data", data);
            comando.Parameters.AddWithValue("@horario", agendamento.Horario);
            comando.Parameters.AddWithValue("@observacoes", agendamento.Observacoes);
            comando.Parameters.AddWithValue("@clinica", idClinica);

            comando.ExecuteNonQuery();
            conexao.Close();
        }

        public List<string> ListarHorariosDisponiveis(DateOnly data, int idUsuario)
        {
            var conexao = ConnectionFactory.Build();
            conexao.Open();

            var query = @"SELECT Horario FROM Agendamento WHERE DataDaConsulta = @data AND Dentista = @id";

            var comando = new MySqlCommand(query, conexao);
            comando.Parameters.AddWithValue("@data", data.ToString("yyyy-MM-dd"));
            comando.Parameters.AddWithValue("@id", idUsuario);

            var dataReader = comando.ExecuteReader();

            var horariosIndisponiveis = new List<TimeOnly>();

            while (dataReader.Read())
            {
                var horario = dataReader.GetTimeSpan(0);
                var horaIndisponivel = new TimeOnly(horario.Hours, horario.Minutes, horario.Seconds);
                horariosIndisponiveis.Add(horaIndisponivel);
            }
            conexao.Close();

            var horariosDisponiveis = new List<string>();
            var horarioInicial = new TimeOnly(08, 00, 00);
            var horarioFinal = new TimeOnly(20, 00, 00);

            while (horarioInicial <= horarioFinal)
            {
                if(horariosIndisponiveis.Contains(horarioInicial) == false)
                {
                    horariosDisponiveis.Add(horarioInicial.ToString("HH:mm"));
                }
                horarioInicial = horarioInicial.AddMinutes(30);
            }

            return horariosDisponiveis;
        }

        public bool VerificarAgendamento(CriarAgendamentoDTO agendamento, int idClinica)
        {
            var data = $"{agendamento.Ano}/{agendamento.Mes}/{agendamento.Dia}";

            //COLOCAR CLINICA
            var conexao = ConnectionFactory.Build();
            conexao.Open();

            var query = "SELECT * FROM Agendamento WHERE DataDaConsulta = @data AND Horario = @horario";

            var comando = new MySqlCommand(query, conexao);

            comando.Parameters.AddWithValue("@data", data);
            comando.Parameters.AddWithValue("@horario", agendamento.Horario);

            var dataReader = comando.ExecuteReader();


            bool agendamentoEncontrado = false;


            if (dataReader.Read())
            {

                agendamentoEncontrado = true;
            }
            conexao.Close();


            return agendamentoEncontrado;
        }
    }
}
