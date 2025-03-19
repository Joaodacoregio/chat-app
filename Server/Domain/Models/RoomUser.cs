using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Intrinsics.X86;

namespace chatApp.Server.Domain.Models
{
    public class RoomUser
    {
        [Key, Column(Order = 0)]
        public int RoomId { get; set; } // Parte da chave primária composta e FK para Room

        [Key, Column(Order = 1)]
        public string UserId { get; set; } // Parte da chave primária composta e FK para User

        public virtual Room Room { get; set; } = default!; // Propriedade de navegação
        public virtual User User { get; set; } = default!; // Propriedade de navegação

        //public virtual User User e public virtual Room (Usado por ef)
        //Room são propriedades de navegação que permitem acessar as entidades User e Room a partir de RoomUser.
    }
}



//Essa tabela foi introduzida para gerenciar o relacionamento muitos-para-muitos (N:N)
//colunas não suportam listas de valores (ex.: uma coluna em Rooms com vários UserId).
//Para resolver isso, vou usar uma tabela intermediaria chamada RoomUser.
//Vai dar para criar consultas melhores pois com ela vai acontecer um mapeamento para usuarios e salas.

//JEITO RUIM!
//--Supondo uma coluna "Users" em Rooms com IDs separados por vírgula (ex.: "1,2,3")
//SELECT RoomId, Users
//FROM Rooms
//WHERE Users LIKE '%2%'; --Procurar salas onde o User 2 está

//Viola a 1NF (valores não atômicos).
//Consultas baseadas em LIKE são lentas e imprecisas (ex.: "2" pode ser confundido com "12").
//Não suporta índices eficientes.
//Dificulta a integridade referencial (sem FKs).

// ================================================

//SELECT r.RoomId, r.UsersMask, r.CreatedAt
//FROM Rooms r
//INNER JOIN RoomUsers ru ON r.RoomId = ru.RoomId
//WHERE ru.UserId = 2;