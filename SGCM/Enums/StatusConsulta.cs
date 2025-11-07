namespace SGCM.Enums
{
    /// <summary>
    /// Enumeração que representa os possíveis status de uma consulta
    /// </summary>
    public enum StatusConsulta
    {
        /// <summary>
        /// Consulta agendada, aguardando realização
        /// </summary>
        Agendada,

        /// <summary>
        /// Consulta realizada e concluída
        /// </summary>
        Concluida,

        /// <summary>
        /// Consulta cancelada antes da realização
        /// </summary>
        Cancelada
    }
}