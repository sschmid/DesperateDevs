using DesperateDevs.Utils;
using NSpec;
using Shouldly;

class describe_PublicMemberInfo : nspec {

    void when_creating() {

        context["when getting public member infos"] = () => {

            it["creates empty info when class has no fields or properties"] = () => {
                var infos = typeof(TestClass).GetPublicMemberInfos();
                infos.ShouldBeEmpty();
            };

            it["creates member infos for public fields"] = () => {
                var infos = typeof(PublicMemberInfoTestClassWithFields).GetPublicMemberInfos();
                infos.Count.ShouldBe(1);
                var mi = infos[0];
                mi.type.ShouldBe(typeof(string));
                mi.name.ShouldBe("publicField");
                mi.attributes.Length.ShouldBe(1);
                mi.attributes[0].memberInfos.Count.ShouldBe(1);
                var attrValue = mi.attributes[0].memberInfos[0].GetValue(mi.attributes[0].attribute);
                attrValue.ShouldBe("myField");
            };

            it["creates member infos for public properties (read & write)"] = () => {
                var infos = typeof(PublicMemberInfoTestClassWithProperties).GetPublicMemberInfos();
                infos.Count.ShouldBe(1);
                var mi = infos[0];
                mi.type.ShouldBe(typeof(string));
                mi.name.ShouldBe("publicProperty");
                mi.attributes.Length.ShouldBe(1);
                mi.attributes[0].memberInfos.Count.ShouldBe(1);
                var attrValue = mi.attributes[0].memberInfos[0].GetValue(mi.attributes[0].attribute);
                attrValue.ShouldBe("myProperty");
            };

            it["creates member infos for fields and properties"] = () => {
                var infos = typeof(PublicMemberInfoTestClassWithFieldsAndProperties).GetPublicMemberInfos();
                infos.Count.ShouldBe(2);
                var mi1 = infos[0];
                var mi2 = infos[1];

                mi1.type.ShouldBe(typeof(string));
                mi1.name.ShouldBe("publicField");

                mi2.type.ShouldBe(typeof(string));
                mi2.name.ShouldBe("publicProperty");
            };

            it["gets values for fields and properties"] = () => {
                var obj = new PublicMemberInfoTestClassWithFieldsAndProperties();
                obj.publicField = "publicFieldValue";
                obj.publicProperty = "publicPropertyValue";

                var infos = obj.GetType().GetPublicMemberInfos();
                var mi1 = infos[0];
                var mi2 = infos[1];

                mi1.GetValue(obj).ShouldBe("publicFieldValue");
                mi2.GetValue(obj).ShouldBe("publicPropertyValue");
            };

            it["sets values for fields and properties"] = () => {
                var obj = new PublicMemberInfoTestClassWithFieldsAndProperties();

                var infos = obj.GetType().GetPublicMemberInfos();
                var mi1 = infos[0];
                var mi2 = infos[1];

                mi1.SetValue(obj, "publicFieldValue");
                mi2.SetValue(obj, "publicPropertyValue");

                obj.publicField.ShouldBe("publicFieldValue");
                obj.publicProperty.ShouldBe("publicPropertyValue");
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

                clone.ShouldNotBeSameAs(obj);
                clone.publicField.ShouldBe(obj.publicField);
                clone.publicProperty.ShouldBe(obj.publicProperty);
            };

            it["copies public members to other obj"] = () => {
                var newObj = new PublicMemberInfoTestClassWithFieldsAndProperties();

                obj.CopyPublicMemberValues(newObj);

                newObj.publicField.ShouldBe(obj.publicField);
                newObj.publicProperty.ShouldBe(obj.publicProperty);
            };
        };
    }
}
