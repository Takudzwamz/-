using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Faculty.Models
{
	[System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public class MetadataAttribute : Attribute//, IModelMetadataAware
	{

        public bool ShowInList { get; set; } = true; // versh



        //public void GetDisplayMetadata(DisplayMetadataProviderContext context) //IModelMetadataAware
        //{
        //    context.DisplayMetadata.AdditionalValues.Add("ShowInList", ShowInList);
        //}

        //public static T CreateFromBase<T>(EntityMetadataAttribute parent) where T : EntityMetadataAttribute, new()
        //{
        //    if (parent != null)
        //    {
        //        var child = new T();
        //        child.DisplayName = parent.DisplayName;
        //        child.Order = parent.Order;
        //        child.Skip = parent.Skip;
        //        child.Required = parent.Required;
        //        return child;
        //    }
        //    return null;
        //}

        //public void OnMetadataCreated(ModelMetadata metadata)
        //{
        //    if (metadata == null)
        //    {
        //        throw new ArgumentNullException("metadata");
        //    }
        //    if (IsRequired.HasValue)
        //    {
        //        metadata.IsRequired = IsRequired.Value;
        //    }
        //    if (Watermark != null)
        //    {
        //        metadata.Watermark = Watermark;
        //    }
        //    metadata.ShowForDisplay = ShowForDisplay;
        //    metadata.ShowForEdit = ShowForEdit;
        //    metadata.HideSurroundingHtml = HideSurroundingHtml;
        //    metadata.AdditionalValues.Add("ShowInList", ShowInList);// versh
        //    if (ColumnClass != null) metadata.AdditionalValues.Add("ColumnClass", ColumnClass);// versh
        //}
    }


    //????????????????
    //http://blog.emikek.com/reinstating-imetadataaware-in-asp-net-5-vnext-mvc-6/
    public interface IModelMetadataAware
	{
		void GetDisplayMetadata(DisplayMetadataProviderContext context);
	}
    public class CustomMetadataProvider : IDisplayMetadataProvider
    {
        public CustomMetadataProvider() { }

        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            if (context.PropertyAttributes != null)
            {

                foreach (object propAttr in context.PropertyAttributes)
                {
                    MetadataAttribute addMetaAttr = propAttr as MetadataAttribute;
                    if (addMetaAttr != null)
                    {
                        context.DisplayMetadata.AdditionalValues.Add
                                      ("ShowInList", addMetaAttr.ShowInList);
                    }
                }
            }
        }
    }
    //public class MyModelMetadataProvider : IDisplayMetadataProvider
    //{
    //    private static List<Type> Registry = new List<Type>();
    //    public static void RegisterMetadataAwareAttribute(Type Attribute)
    //    {
    //        if (Attribute == null)
    //        {
    //            throw new ArgumentNullException();
    //        }

    //        if (!typeof(IModelMetadataAware).IsAssignableFrom(Attribute))
    //        {
    //            throw new ArgumentException("This attribute type is not metadata aware.");
    //        }

    //        Registry.Add(Attribute);
    //    }

    //    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    //    {
    //        if (context == null)
    //        {
    //            throw new ArgumentNullException();
    //        }

    //        foreach (Type Type in Registry)
    //        {
    //            IModelMetadataAware Attribute = (IModelMetadataAware)context.Attributes
    //                .Where(x => x.GetType() == Type)
    //                .FirstOrDefault();

    //            if (Attribute != null)
    //            {
    //                Attribute.GetDisplayMetadata(context);
    //            }
    //        }
    //    }

    //}


}
