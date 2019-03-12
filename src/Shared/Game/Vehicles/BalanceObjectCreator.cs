using System;
using Urho;
using Urho.Urho2D;

namespace SmartRoadSense.Shared {
    public static class BalanceObjectCreator {

        public static Node CreateBalanceObject(Game GameInstance, BaseScene scene, string objectName)
        {

            Node node = scene.CreateChild(objectName);
            node.Position = (new Vector3(-0.51f, -0.39f, 3.0f));
            node.Scale = new Vector3(1.0f, 1.0f, 0.0f);

            // Create rigid body
            RigidBody2D body = node.CreateComponent<RigidBody2D>();
            body.BodyType = BodyType2D.Dynamic;

            // TODO: select proper balance object
            Sprite2D boxSprite = GameInstance.ResourceCache.GetSprite2D(AssetsCoordinates.Level.BalanceObjects.ResourcePath);
            boxSprite.Rectangle = AssetsCoordinates.Level.BalanceObjects.JukeBox;
            //Sprite2D boxSprite = GameInstance.ResourceCache.GetSprite2D("Urho2D/Box.png");
            StaticSprite2D staticSprite = node.CreateComponent<StaticSprite2D>();
            staticSprite.Sprite = boxSprite;

            // Create box
            CollisionBox2D box = node.CreateComponent<CollisionBox2D>();
            // Set size
            box.Size = new Vector2(0.32f, 0.32f);
            // Set density
            box.Density = 4.0f;
            // Set friction
            box.Friction = 1.0f;
            // Set restitution
            box.Restitution = 0.1f;

            return node;
        }
    }
}
