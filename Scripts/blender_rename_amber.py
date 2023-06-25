import bpy

translation_dict = {
    "センター": "Center",
    "グルーブ": "Groove",
    "スカート_0_1": "Skirt_0_1",
    "スカート_1_1": "Skirt_1_1",
    "スカート_2_1": "Skirt_2_1",
    "スカート_3_1": "Skirt_3_1",
    "スカート_4_1": "Skirt_4_1",
    "スカート_5_1": "Skirt_5_1",
    "スカート_6_1": "Skirt_6_1",
    "スカート_7_1": "Skirt_7_1",
    "スカート_8_1": "Skirt_8_1",
    "安柏_mesh": "Amber_mesh",
    "操作中心": "Control_Center",
    "全ての親": "All_Parents",
    "皮肤": "Skin",
    "脸": "Face",
    "眼白": "Sclera",
    "头发": "Hair",
    "表情": "Expression",
    "眼睛": "Eyes",
    "衣服": "Clothes",
    "头饰": "Head_Decoration",
    "腰": "Hips",
    "上半身": "Upper_Body",
    "上半身2": "Upper_Body2",
    "首": "Neck",
    "頭": "Head",
    "右目": "Right_Eye",
    "右目戻": "Right_Eye_Return",
    "左目": "Left_Eye",
    "左目戻": "Left_Eye_Return",
    "両目": "Both_Eyes",
    "右肩P": "Right_ShoulderP",
    "右肩": "Right_Shoulder",
    "右肩C": "Right_ShoulderC",
    "右腕": "Right_Arm",
    "右腕捩": "Right_Arm_Twist",
    "右ひじ": "Right_Elbow",
    "右手捩": "Right_Hand_Twist",
    "右手首": "Right_Wrist",
    "右ダミー": "Right_Dummy",
    "右手先": "Right_Hand_End",
    "右親指０": "Right_Thumb0",
    "右親指１": "Right_Thumb1",
    "右親指２": "Right_Thumb2",
    "右親指先": "Right_Thumb_Tip",
    "右人指１": "Right_Index1",
    "右人指２": "Right_Index2",
    "右人指３": "Right_Index3",
    "右人指先": "Right_Index_Tip",
    "右中指１": "Right_Middle1",
    "右中指２": "Right_Middle2",
    "右中指３": "Right_Middle3",
    "右中指先": "Right_Middle_Tip",
    "右薬指１": "Right_Ring1",
    "右薬指２": "Right_Ring2",
    "右薬指３": "Right_Ring3",
    "右薬指先": "Right_Ring_Tip",
    "右小指１": "Right_Pinky1",
    "右小指２": "Right_Pinky2",
    "右小指３": "Right_Pinky3",
    "右小指先": "Right_Pinky_Tip",
    "右手捩1": "Right_Hand_Twist1",
    "右手捩2": "Right_Hand_Twist2",
    "右手捩3": "Right_Hand_Twist3",
    "右腕捩1": "Right_Arm_Twist1",
    "右腕捩2": "Right_Arm_Twist2",
    "右腕捩3": "Right_Arm_Twist3",
    "右ひじ補助": "Right_Elbow_Auxiliary",
    "+右ひじ補助": "+Right_Elbow_Auxiliary",
    "左肩P": "Left_ShoulderP",
    "左肩": "Left_Shoulder",
    "左肩C": "Left_ShoulderC",
    "左腕": "Left_Arm",
    "左腕捩": "Left_Arm_Twist",
    "左ひじ": "Left_Elbow",
    "左手捩": "Left_Hand_Twist",
    "左手首": "Left_Wrist",
    "左ダミー": "Left_Dummy",
    "左手先": "Left_Hand_End",
    "左親指０": "Left_Thumb0",
    "左親指１": "Left_Thumb1",
    "左親指２": "Left_Thumb2",
    "左親指先": "Left_Thumb_Tip",
    "左人指１": "Left_Index1",
    "左人指２": "Left_Index2",
    "左人指３": "Left_Index3",
    "左人指先": "Left_Index_Tip",
    "左中指１": "Left_Middle1",
    "左中指２": "Left_Middle2",
    "左中指３": "Left_Middle3",
    "左中指先": "Left_Middle_Tip",
    "左薬指１": "Left_Ring1",
    "左薬指２": "Left_Ring2",
    "左薬指３": "Left_Ring3",
    "左薬指先": "Left_Ring_Tip",
    "左小指１": "Left_Pinky1",
    "左小指２": "Left_Pinky2",
    "左小指３": "Left_Pinky3",
    "左小指先": "Left_Pinky_Tip",
    "左手捩1": "Left_Hand_Twist1",
    "左手捩2": "Left_Hand_Twist2",
    "左手捩3": "Left_Hand_Twist3",
    "左腕捩1": "Left_Arm_Twist1",
    "左腕捩2": "Left_Arm_Twist2",
    "左腕捩3": "Left_Arm_Twist3",
    "左ひじ補助": "Left_Elbow_Auxiliary",
    "+左ひじ補助": "+Left_Elbow_Auxiliary",
    "おっぱい調整": "Breast_Adjustment",
    "左胸上": "Left_Chest_Upper",
    "左胸上2": "Left_Chest_Upper2",
    "左胸先": "Left_Chest_Tip",
    "左胸下": "Left_Chest_Lower",
    "左胸下先": "Left_Chest_Lower_Tip",
    "右胸上": "Right_Chest_Upper",
    "右胸上2": "Right_Chest_Upper2",
    "右胸先": "Right_Chest_Tip",
    "右胸下": "Right_Chest_Lower",
    "右胸下先": "Right_Chest_Lower_Tip",
    "下半身": "Lower_Body",
    "腰キャンセル右": "Hips_Cancel_Right",
    "右足": "Right_Leg",
    "右ひざ": "Right_Knee",
    "右足首": "Right_Ankle",
    "右つま先": "Right_Toe",
    "右足D": "Right_LegD",
    "右ひざD": "Right_KneeD",
    "右足首D": "Right_AnkleD",
    "右足先EX": "Right_Foot_TipEX",
    "腰キャンセル左": "Hips_Cancel_Left",
    "左足": "Left_Leg",
    "左ひざ": "Left_Knee",
    "左足首": "Left_Ankle",
    "左つま先": "Left_Toe",
    "左足D": "Left_LegD",
    "左ひざD": "Left_KneeD",
    "左足首D": "Left_AnkleD",
    "左足先EX": "Left_Foot_TipEX",
    "腰パーツ親": "Hips_Part_Parent",
    "右足IK親": "Right_LegIK_Parent",
    "右足ＩＫ": "Right_Leg_IK",
    "右つま先ＩＫ": "Right_Toe_IK",
    "左足IK親": "Left_LegIK_Parent",
    "左足ＩＫ": "Left_Leg_IK",
    "左つま先ＩＫ": "Left_Toe_IK",
    "センター2": "Center2",
    "グルーブ2": "Groove2",
    "左目先": "LeftEyeEnd",
    "右目先": "RightEyeEnd",
    "耳坠1": "Earring1",
    "舌１": "Tongue1",
    "舌２": "Tongue2",
    "白目": "WhiteEye",
    "眉": "Eyebrow",
    "二重": "Doublefold",
    "目星": "EyeStar",
    "发": "Hair",
    "服1": "Clothes1",
    "神之眼": "EyeOfGod",
    "耳坠": "Earring",
    "面": "Face",
    "睫": "Lashes",
    "口齿": "MouthTeeth",
    "目": "Eye",
    "服2": "Clothes2",
    "饰": "Decoration",
    "照れ": "Blush"
}

# The following keys follow a pattern and can be generated with a loop
for i in range(10):
    translation_dict.update({f"スカート_{i}_1": f"Skirt_{i}_1"})

for i in range(12):
    for j in range(10):
        translation_dict.update({f"衣_{i}_{j}": f"Clothes_{i}_{j}"})

for i in range(5):
    translation_dict.update({f"耳坠_{i}_1": f"Earring_{i}_1"})

for i in range(10):
    translation_dict.update({f"发_{i}_1": f"Hair_{i}_1"})

for i in range(6):
    translation_dict.update({f"侧发_{i}_1": f"SideHair_{i}_1"})
    
for i in range(6):
    translation_dict.update({f"饰_{i}_1": f"Decoration_{i}_1"})

def rename_obj(obj, translation_dict):
    if obj.name in translation_dict:
        obj.name = translation_dict[obj.name]

    if obj.type == 'ARMATURE':
        for bone in obj.data.bones:
            if bone.name in translation_dict:
                bone.name = translation_dict[bone.name]

def rename_objects_in_hierarchy(obj, translation_dict):
    rename_obj(obj, translation_dict)
    for child in obj.children:
        rename_objects_in_hierarchy(child, translation_dict)

# Clear the console
bpy.ops.wm.console_toggle()

# Rename the objects in the hierarchy
for obj in bpy.data.objects:
    rename_objects_in_hierarchy(obj, translation_dict)