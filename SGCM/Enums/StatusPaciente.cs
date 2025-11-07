namespace SGCM.Enums
{
    /// <summary>
    /// Enumeração que representa os possíveis status de um paciente
    /// </summary>
    public enum StatusPaciente
    {
        /// <summary>
        /// Paciente ativo no sistema, pode agendar consultas
        /// </summary>
        Ativo,

        /// <summary>
        /// Paciente inativo, não pode agendar novas consultas
        /// </summary>
        Inativo
    }
}