using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq.Expressions;

namespace Faculty.Models
{
    public interface IEntityBase
    {
        int Id { get; set; }
        string Name { get; set; }
        int GetKeyValue();
        bool IsNew();
        string GetClassName();
        object GetPropertyValue(string propertyName);
        void SetPropertyValue(string propertyName, object value);
        object Clone();
        AttrType GetModelAttribute<AttrType>() where AttrType : Attribute;
        AttrType GetPropertyAttribute<AttrType>(string propName) where AttrType : Attribute;

    }

    /// <summary>
    /// Базовый класс для всех сущностей в <see cref="DataModel">модели данных</see>
    /// </summary>
    public abstract class EntityBase : ICloneable, IEntityBase//, IValidatableObject
    {
        [MaxLength(255)]
        [Required]
        [Display(Name = "Наименование", Order = 1)]
        // [UIHint("TextField")]
        [ScaffoldColumn(false)]
        public virtual string Name { get; set; } // ScaffoldColumn(false) чтобы не создавать поле в @Html.EditorForModel(), так как в виде (Edit, Create) я своё вставил

        public override string ToString()
        {
            if (GetType().GetProperty("Name") != null)
            { return (string)GetPropertyValue("Name"); }
            else { return base.ToString(); }
        }

        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[UIHint("Id")]
        [HiddenInput(DisplayValue = false)]
        [ScaffoldColumn(false)]
        // [Metadata(HideSurroundingHtml = true)]
        [Display(Name = "Id", Order = 0)]
        public virtual int Id { get; set; } = 0;

        /// <summary>
        /// Получение значения ключа
        /// </summary>
        /// <returns>Значение ключа</returns>
        public virtual int GetKeyValue()
        {
            return Id;
        }
        /// <summary>
        /// Признак: новый ли экземпляр (не сохраненный в БД)
        /// </summary>
        /// <returns>Не сохранен в БД</returns>
        public bool IsNew()
        {
            return Id == 0;
        }



        //[StringLength(50)]
        //[Metadata(ShowForDisplay = false, ShowForEdit = false)]
        //public string ExternalId { get; set; }  

        #region Metadata
        private ModelMetadata GetMetadata()
        {
            return new EmptyModelMetadataProvider().GetMetadataForType(GetType());
        }

        /// <summary>
        /// Получение имени класса
        /// </summary>
        /// <returns>Имя класса</returns>
        public string GetClassName()
        {
            return GetMetadata().ModelType.Name;
        }


        /// <summary>
        /// Получение значения свойства объекта по имени свойства
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        /// <returns>Значение свойства</returns>
        public object GetPropertyValue(string propertyName)
        {
            var type = GetType();
            var p = type.GetProperty(propertyName);
            if (p != null)
            {
                return p.GetValue(this);
            }
            return null;
        }

        /// <summary>
        /// Установка значения свойства объекта по имени свойства
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        /// <param name="value">Новое значение свойства</param>
        public void SetPropertyValue(string propertyName, object value)
        {
            var type = GetType();
            var p = type.GetProperty(propertyName);
            if (p != null)
            {
                if (!p.CanWrite)
                {
                    return;
                }
                //p.SetValue(this, value);
                TypeConverter converter = TypeDescriptor.GetConverter(p.PropertyType);
                if (converter.CanConvertFrom(value.GetType()))
                {
                    var a = converter.ConvertFrom(value);
                    p.SetValue(this, a);
                }
                else
                {
                    // Если  не получилось сконвертить через TypeConverter
                    var a = Convert.ChangeType(value, p.PropertyType);
                    p.SetValue(this, a);
                }
            }

        }
        #endregion


        public object Clone()
        {
            return MemberwiseClone();
        }
        public AttrType GetModelAttribute<AttrType>() where AttrType: Attribute
        {
            var attr = GetType().GetCustomAttributes(typeof(AttrType), true).FirstOrDefault() as AttrType;
            return attr;
        }
        public AttrType GetPropertyAttribute<AttrType>(string propName) where AttrType : Attribute
        {
            var attr =  GetType().GetProperty(propName).GetCustomAttributes(typeof(AttrType), true).FirstOrDefault() as AttrType;
            return attr;
        }


    }
}
