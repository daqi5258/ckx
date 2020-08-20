/*
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows.OPM;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace OPMNetSample
{
    #region Our Custom Property
    [
        Guid("F60AE3DA-0373-4d24-82D2-B2646517ABCB"),
        ProgId("OPMNetSample.CustomProperty.1"),

        // No class interface is generated for this class and
        // no interface is marked as the default.
        // Users are expected to expose functionality through
        // interfaces that will be explicitly exposed by the object
        // This means the object can only expose interfaces we define

        ClassInterface(ClassInterfaceType.None),
        // Set the default COM interface that will be used for
        // Automation. Languages like: C#, C++ and VB allow to 
        //query for interface's we're interested in but Automation 
        // only aware languages like javascript do not allow to 
        // query interface(s) and create only the default one

        ComDefaultInterface(typeof(IDynamicProperty2)),
        ComVisible(true)
    ]
    public class CustomProp : IDynamicProperty2 {
        private IDynamicPropertyNotify2 m_pSink =null;

       // Unique property ID
        void IDynamicProperty2.GetGUID(out Guid propGUID)
        {
            propGUID = new Guid("F60AE3DA-0373-4d24-82D2-B2646517ABCB");
        }

        // Property display name
        void IDynamicProperty2.GetDisplayName(out string szName)
        {
            szName = "�Զ�������";
        }

        // Show/Hide property in the OPM, for this object instance
        void IDynamicProperty2.IsPropertyEnabled(object pUnk, out int bEnabled)
        {
            bEnabled = 1;
        }

        // Is property showing but disabled
        void IDynamicProperty2.IsPropertyReadOnly(out int bReadonly)
        {
            bReadonly = 0;
        }

        // Get the property description string
        void IDynamicProperty2.GetDescription(out string szName)
        {
            szName = "��������";
        }

        // OPM will typically display these in an edit field
        // optional: meta data representing property type name,
        // ex. ACAD_ANGLE
        void IDynamicProperty2.GetCurrentValueName(out string szName)
        {
            throw new System.NotImplementedException();
        }

        // What is the property type, ex. VT_R8
        void IDynamicProperty2.GetCurrentValueType(out ushort varType)
        {
            // The Property Inspector supports the following data
            // types for dynamic properties:
            // VT_I2, VT_I4, VT_R4, VT_R8,VT_BSTR, VT_BOOL
            // and VT_USERDEFINED. 

            varType = 4; // VT_I4
        }

        // Get the property value, passes the specific object
        // we need the property value for.
        void IDynamicProperty2.GetCurrentValueData(object pUnk, ref object pVarData)
        {
            // TODO: Get the value and return it to AutoCAD

            // Because we said the value type was a 32b int (VT_I4)
            if (pVarData==null)
            {
                pVarData = "";
            }else
                pVarData = pVarData;

        }

        // Set the property value, passes the specific object we
        // want to set the property value for
        void IDynamicProperty2.SetCurrentValueData(object pUnk, object varData)
        {
            // TODO: Save the value returned to you

            // Because we said the value type was a 32b int (VT_I4)
            Object myVal = varData;
        }

        // OPM passes its implementation of IDynamicPropertyNotify, you
        // cache it and call it to inform OPM your property has changed
        void IDynamicProperty2.Connect(object pSink)
        {
            m_pSink = (IDynamicPropertyNotify2)pSink;
        }

        void IDynamicProperty2.Disconnect()
        {
            m_pSink = null;
        }

    }
    #endregion

    /*#region Application Entry Point
    public class MyEntryPoint : IExtensionApplication {
        protected internal CustomProp custProp = null;

        public void Initialize () {
            Assembly.LoadFrom("asdkOPMNetExt64.dll");

            // Add the Dynamic Property
            Dictionary classDict =SystemObjects.ClassDictionary;
            RXClass lineDesc =(RXClass)classDict.At("AcDbLine");
            IPropertyManager2 pPropMan =(IPropertyManager2)xOPM.xGET_OPMPROPERTY_MANAGER(lineDesc);

            custProp =new CustomProp();
            pPropMan.AddProperty((object)custProp);
        }

        public void Terminate () {
            // Remove the Dynamic Property
            Dictionary classDict =SystemObjects.ClassDictionary;
            RXClass lineDesc =(RXClass)classDict.At("AcDbLine");
            IPropertyManager2 pPropMan =(IPropertyManager2)xOPM.xGET_OPMPROPERTY_MANAGER(lineDesc);

            pPropMan.RemoveProperty((object)custProp);
            custProp =null;
        }

    }
    #endregion*/
/*

}
*/