using DesperateDevs.Utils;
using NSpec;

class describe_PublicMemberInfo : nspec {

    void when_creating() {

        context["when getting public member infos"] = () => {

            it["creates empty info when class has no fields or properties"] = () => {
                var infos = typeof(TestClass).GetPublicMemberInfos();
                infos.should_be_empty();
            };

            it["creates member infos for public fields"] = () => {
                var infos = typeof(PublicMemberInfoTestClassWithFields).GetPublicMemberInfos();
                infos.Count.should_be(1);
                var mi = infos[0];
                mi.type.should_be(typeof(string));
                mi.name.should_be("publicField");
                mi.attributes.Length.should_be(1);
                mi.attributes[0].memberInfos.Count.should_be(1);
                var attrValue = mi.attributes[0].memberInfos[0].GetValue(mi.attributes[0].attribute);
                attrValue.should_be("myField");
            };

            it["creates member infos for public properties (read & write)"] = () => {
                var infos = typeof(PublicMemberInfoTestClassWithProperties).GetPublicMemberInfos();
                infos.Count.should_be(1);
                var mi = infos[0];
                mi.type.should_be(typeof(string));
                mi.name.should_be("publicProperty");
                mi.attributes.Length.should_be(1);
                mi.attributes[0].memberInfos.Count.should_be(1);
                var attrValue = mi.attributes[0].memberInfos[0].GetValue(mi.attributes[0].attribute);
                attrValue.should_be("myProperty");
            };

            it["creates member infos for fields and properties"] = () => {
                var infos = typeof(PublicMemberInfoTestClassWithFieldsAndProperties).GetPublicMemberInfos();
                infos.Count.should_be(2);
                var mi1 = infos[0];
                var mi2 = infos[1];

                mi1.type.should_be(typeof(string));
                mi1.name.should_be("publicField");

                mi2.type.should_be(typeof(string));
                mi2.name.should_be("publicProperty");
            };

            it["gets values for fields and properties"] = () => {
                var obj = new PublicMemberInfoTestClassWithFieldsAndProperties();
                obj.publicField = "publicFieldValue";
                obj.publicProperty = "publicPropertyValue";

                var infos = obj.GetType().GetPublicMemberInfos();
                var mi1 = infos[0];
                var mi2 = infos[1];

                mi1.GetValue(obj).should_be("publicFieldValue");
                mi2.GetValue(obj).should_be("publicPropertyValue");
            };

            it["sets values for fields and properties"] = () => {
                var obj = new PublicMemberInfoTestClassWithFieldsAndProperties();

                var infos = obj.GetType().GetPublicMemberInfos();
                var mi1 = infos[0];
                var mi2 = infos[1];

                mi1.SetValue(obj, "publicFieldValue");
                mi2.SetValue(obj, "publicPropertyValue");

                obj.publicField.should_be("publicFieldValue");
                obj.publicProperty.should_be("publicPropertyValue");
            };
        };

        context["when cloning object"] = () => {

            PublicMemberInfoTestClassWithFieldsAndProperties obj = null;

            before = () => {
                obj = new PublicMemberInfoTestClassWithFieldsAndProperties();
                obj.publicField = "field";
                obj.publicProperty = "property";
            };

            it["clones object and sets public members"] = () => {
                var clone = (PublicMemberInfoTestClassWithFieldsAndProperties)obj.PublicMemberClone();

                clone.should_not_be_same(obj);
                clone.publicField.should_be(obj.publicField);
                clone.publicProperty.should_be(obj.publicProperty);
            };

            it["copies public members to other obj"] = () => {
                var newObj = new PublicMemberInfoTestClassWithFieldsAndProperties();

                obj.CopyPublicMemberValues(newObj);

                newObj.publicField.should_be(obj.publicField);
                newObj.publicProperty.should_be(obj.publicProperty);
            };
        };
    }
}
