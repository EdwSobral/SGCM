namespace SGCM.Enums
{
    /// <summary>
    /// Enumeração que representa os possíveis status de um médico
    /// </summary>
    public enum StatusMedico
    {
        /// <summary>
        /// Médico ativo no sistema, pode receber consultas
        /// </summary>
        Ativo,

        /// <summary>
        /// Médico inativo, não pode receber novas consultas
        /// </summary>
        Inativo
    }
}