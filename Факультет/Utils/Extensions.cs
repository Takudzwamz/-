using Faculty.Data;
using Faculty.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Faculty.Utils
{
    public static class Extensions
    {
        public static Exception GetInnerMostException(this Exception ex)
        {
            Exception currentEx = ex;
            while (currentEx.InnerException != null)
            {
                currentEx = currentEx.InnerException;
            }

            return currentEx;
        }

        public static AttrType GetTypeAttribute<AttrType>(this IHtmlHelper helper, Type t)
        {
            var ta = ((DefaultModelMetadata)helper.MetadataProvider.GetMetadataForType(t)).Attributes.TypeAttributes;
            return (AttrType)ta.FirstOrDefault(attr => typeof(AttrType).IsInstanceOfType(attr));
        }
        public static AttrType GetPropertyAttribute<AttrType>(this IHtmlHelper helper, Type t, string propName) //??????? не доделал
        {
            var pmd = helper.MetadataProvider.GetMetadataForProperties(t).FirstOrDefault(p => p.PropertyName == propName);
            return (AttrType)((DefaultModelMetadata)pmd).Attributes.PropertyAttributes.FirstOrDefault(attr => typeof(AttrType).IsInstanceOfType(attr));
        }



        public static IQueryable<IEntityBase> GetContextByName(this ApplicationDbContext context, string entityName)
        {
            var tType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name == entityName);
            if (tType != null)
            {
                // вызов метода Set<T>, где T задан строкой entityName
                var SetMethod = typeof(ApplicationDbContext).GetMethod("Set");
                var Set_T_Method = SetMethod.MakeGenericMethod(new[] { tType });
                var dbSet = Set_T_Method.Invoke(context, null);
                return (IQueryable<IEntityBase>)dbSet;
            }
            else
            {
                throw new NotImplementedException("Не найден тип сущности контекста:" + entityName);
            }
        }

        #region ViewData

        //public static IDataModel Context(this ViewDataDictionary viewData)
        //{
        //	return DependencyResolver.Current.GetService<IDataModel>();
        //}
        public static ModelMetadata CollectionModelMetadata(this ViewDataDictionary viewData)
        {
            return viewData.CollectionModelMetadata(new EmptyModelMetadataProvider());
        }

        public static ModelMetadata CollectionModelMetadata(this ViewDataDictionary viewData, ModelMetadataProvider provider)
        {
            var type = viewData.ModelMetadata.ModelType.GenericTypeArguments.FirstOrDefault();
            return provider.GetMetadataForType(type);
        }
        public static ModelMetadata CollectionModelMetadata(this ViewDataDictionary viewData, Type type)
        {
            return viewData.CollectionModelMetadata(new EmptyModelMetadataProvider(), type);
        }
        public static ModelMetadata CollectionModelMetadata(this ViewDataDictionary viewData, ModelMetadataProvider provider, Type type)
        {
            return provider.GetMetadataForType(type);
        }
        public static ModelMetadata CollectionModelMetadata<T>(this ViewDataDictionary viewData)
        {
            return viewData.CollectionModelMetadata(new EmptyModelMetadataProvider(), typeof(T));
        }
        public static ModelMetadata CollectionModelMetadata<T>(this ViewDataDictionary viewData, ModelMetadataProvider provider)
        {
            return provider.GetMetadataForType(typeof(T));
        }

        #endregion ViewData



    }
}
