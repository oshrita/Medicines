//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Medicine.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Burdeneds
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Burdeneds()
        {
            this.Hospitalizations = new HashSet<Hospitalizations>();
        }
        [Required(ErrorMessage = "���� ����!")]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "���� ����� ��� ������ 0-9")]//����� ��� 0-9
        [MaxLength(9), MinLength(9)]
        //[StringLength(9, ErrorMessage = "���� 9 �����")]
        public string Id { get; set; }

        [Required(ErrorMessage = "���� ����!")]
        [RegularExpression("(^[�-�]+$)", ErrorMessage = "���� ������ ����� ��� �' �-�'")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "���� ����!")]
        [RegularExpression("(^[�-�]+$)", ErrorMessage = "���� ������ ����� ��� �' �-�'")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "���� ����!")]
        [DataType(DataType.Date)]//����� �� ������
        public System.DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "���� ����!")]
        public string Address { get; set; }

        [Required(ErrorMessage = "���� ����!")]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "���� ����� ��� 0-9")]//����� ��� 0-9
        [DataType(DataType.PhoneNumber)]//����� �� ������
        [MaxLength(10), MinLength(10)]
        [StringLength(10, ErrorMessage = "���� 10 �����")]
        public string Phone { get; set; }

        [DataType(DataType.EmailAddress)]//����� �� ������
        public string Email { get; set; }
        public int CodeContact { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Hospitalizations> Hospitalizations { get; set; }
        public virtual Contact Contact { get; set; }
    }
}
