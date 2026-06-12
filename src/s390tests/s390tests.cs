[AttributeUsage(AttributeTargets.Method)]
public class S390xTestAttribute : Attribute
{
    public int    Number      { get; }
    public string Mnemonic    { get; }
    public string Description { get; }
    public S390xTestAttribute(int number, string mnemonic, string description)
    {
        Number      = number;
        Mnemonic    = mnemonic;
        Description = description;
    }
}

public class S390xInstructionTest
{
    // ===== INTEGER ARITHMETIC OPERATIONS (1-10) =====

    [S390xTest(1, "ark", "Add Register (32-bit)")]
    public static int Test_ark()
    {
        int a = 10;
        int b = 5;
        return (a + b) == 15 ? 0 : 1;
    }

    [S390xTest(2, "agrk", "Add Register (64-bit)")]
    public static int Test_agrk()
    {
        long a = 100L;
        long b = 50L;
        return (a + b) == 150L ? 0 : 2;
    }

    [S390xTest(3, "srk", "Subtract Register (32-bit)")]
    public static int Test_srk()
    {
        int a = 20;
        int b = 8;
        return (a - b) == 12 ? 0 : 3;
    }

    [S390xTest(4, "sgrk", "Subtract Register (64-bit)")]
    public static int Test_sgrk()
    {
        long a = 200L;
        long b = 80L;
        return (a - b) == 120L ? 0 : 4;
    }

    [S390xTest(5, "msrkc", "Multiply (32-bit)")]
    public static int Test_msrkc()
    {
        int a = 6;
        int b = 7;
        return (a * b) == 42 ? 0 : 5;
    }

    [S390xTest(6, "msgrkc", "Multiply (64-bit)")]
    public static int Test_msgrkc()
    {
        long a = 10L;
        long b = 12L;
        return (a * b) == 120L ? 0 : 6;
    }

    [S390xTest(7, "afi", "Add Immediate (32-bit)")]
    public static int Test_afi()
    {
        int a = 15;
        return (a + 10) == 25 ? 0 : 7;
    }

    [S390xTest(8, "agfi", "Add Immediate (64-bit)")]
    public static int Test_agfi()
    {
        long a = 150L;
        return (a + 100L) == 250L ? 0 : 8;
    }

    [S390xTest(9, "msfi", "Multiply Immediate (32-bit)")]
    public static int Test_msfi()
    {
        int a = 5;
        return (a * 8) == 40 ? 0 : 9;
    }

    [S390xTest(10, "msgfi", "Multiply Immediate (64-bit)")]
    public static int Test_msgfi()
    {
        long a = 8L;
        return (a * 9L) == 72L ? 0 : 10;
    }

    // ===== LOGICAL OPERATIONS (11-22) =====

    [S390xTest(11, "nrk", "AND Register (32-bit)")]
    public static int Test_nrk()
    {
        int a = 0xFF;
        int b = 0x0F;
        return (a & b) == 0x0F ? 0 : 11;
    }

    [S390xTest(12, "ngrk", "AND Register (64-bit)")]
    public static int Test_ngrk()
    {
        long a = 0xFFL;
        long b = 0x0FL;
        return (a & b) == 0x0FL ? 0 : 12;
    }

    [S390xTest(13, "ork", "OR Register (32-bit)")]
    public static int Test_ork()
    {
        int a = 0xF0;
        int b = 0x0F;
        return (a | b) == 0xFF ? 0 : 13;
    }

    [S390xTest(14, "ogrk", "OR Register (64-bit)")]
    public static int Test_ogrk()
    {
        long a = 0xF0L;
        long b = 0x0FL;
        return (a | b) == 0xFFL ? 0 : 14;
    }

    [S390xTest(15, "xrk", "XOR Register (32-bit)")]
    public static int Test_xrk()
    {
        int a = 0xFF;
        int b = 0xAA;
        return (a ^ b) == 0x55 ? 0 : 15;
    }

    [S390xTest(16, "xgrk", "XOR Register (64-bit)")]
    public static int Test_xgrk()
    {
        long a = 0xFFL;
        long b = 0xAAL;
        return (a ^ b) == 0x55L ? 0 : 16;
    }

    [S390xTest(17, "ncrk", "AND with Complement (32-bit)")]
    public static int Test_ncrk()
    {
        int a = 0xF0;
        int b = 0xFF;
        return ((~a) & b) == 0x0F ? 0 : 17;
    }

    [S390xTest(18, "oill", "OR Immediate Low-Low")]
    public static int Test_oill()
    {
        int a = 0x1200;
        return (a | 0x00FF) == 0x12FF ? 0 : 18;
    }

    [S390xTest(19, "xihf", "XOR Immediate High (32-bit high part)")]
    public static int Test_xihf()
    {
        int a = 0x12345678;
        return (a ^ unchecked((int)0xFF000000)) == unchecked((int)0xED345678) ? 0 : 19;
    }

    [S390xTest(20, "nihf", "AND Immediate High (32-bit high part)")]
    public static int Test_nihf()
    {
        int a = 0x12345678;
        return (a & unchecked((int)0xFF000000)) == 0x12000000 ? 0 : 20;
    }

    [S390xTest(21, "ni", "AND Immediate (memory)")]
    public static int Test_ni()
    {
        int a = 0xFF;
        return (a & 0x0F) == 0x0F ? 0 : 21;
    }

    [S390xTest(22, "xi", "XOR Immediate (memory)")]
    public static int Test_xi()
    {
        int a = 0xFF;
        return (a ^ 0xAA) == 0x55 ? 0 : 22;
    }

    // ===== LOAD OPERATIONS (23-33) =====

    [S390xTest(23, "lgfi", "Load Immediate (64-bit)")]
    public static int Test_lgfi()
    {
        long a = 0x12345678L;
        return a == 0x12345678L ? 0 : 23;
    }

    [S390xTest(24, "iihf", "Insert Immediate High")]
    public static int Test_iihf()
    {
        long a = unchecked((long)0xCAFEBABE00000000L);
        return a == unchecked((long)0xCAFEBABE00000000L) ? 0 : 24;
    }

    [S390xTest(25, "iilf", "Insert Immediate Low")]
    public static int Test_iilf()
    {
        long a = 0x00000000DEADBEEFL;
        return a == 0x00000000DEADBEEFL ? 0 : 25;
    }

    [S390xTest(26, "llc", "Load Logical Character (zero-extend byte)")]
    public static int Test_llc()
    {
        byte a = 0x42;
        int  r = a;
        return r == 0x42 ? 0 : 26;
    }

    [S390xTest(27, "lgb", "Load Byte (64-bit sign-extended)")]
    public static int Test_lgb()
    {
        sbyte a = -1;
        long  r = a;
        return r == -1L ? 0 : 27;
    }

    [S390xTest(28, "lgh", "Load Halfword (64-bit sign-extended)")]
    public static int Test_lgh()
    {
        short a = 0x1234;
        long  r = a;
        return r == 0x1234L ? 0 : 28;
    }

    [S390xTest(29, "llgh", "Load Logical Halfword (zero-extend)")]
    public static int Test_llgh()
    {
        ushort a = 0xFFFF;
        long   r = a;
        return r == 0xFFFFL ? 0 : 29;
    }

    [S390xTest(30, "l", "Load (32-bit)")]
    public static int Test_l()
    {
        int a = 0x12345678;
        return a == 0x12345678 ? 0 : 30;
    }

    [S390xTest(31, "lg", "Load (64-bit)")]
    public static int Test_lg()
    {
        long a = unchecked((long)0x123456789ABCDEF0L);
        return a == unchecked((long)0x123456789ABCDEF0L) ? 0 : 31;
    }

    [S390xTest(32, "ley", "Load Float (short displacement)")]
    public static int Test_ley()
    {
        float a = 3.14f;
        return a == 3.14f ? 0 : 32;
    }

    [S390xTest(33, "ldy", "Load Double (short displacement)")]
    public static int Test_ldy()
    {
        double a = 3.14159;
        return a == 3.14159 ? 0 : 33;
    }

    // ===== STORE OPERATIONS (34-42) =====

    [S390xTest(34, "stc", "Store Character")]
    public static int Test_stc()
    {
        byte a = 0x42;
        return a == 0x42 ? 0 : 34;
    }

    [S390xTest(35, "sth", "Store Halfword")]
    public static int Test_sth()
    {
        short a = 0x1234;
        return a == 0x1234 ? 0 : 35;
    }

    [S390xTest(36, "st", "Store (32-bit)")]
    public static int Test_st()
    {
        int a = 0x12345678;
        return a == 0x12345678 ? 0 : 36;
    }

    [S390xTest(37, "stg", "Store (64-bit)")]
    public static int Test_stg()
    {
        long a = unchecked((long)0x123456789ABCDEF0L);
        return a == unchecked((long)0x123456789ABCDEF0L) ? 0 : 37;
    }

    [S390xTest(38, "std", "Store Double")]
    public static int Test_std()
    {
        double a = 3.14159;
        return a == 3.14159 ? 0 : 38;
    }

    [S390xTest(39, "stey", "Store Float (short displacement)")]
    public static int Test_stey()
    {
        float a = 3.14f;
        return a == 3.14f ? 0 : 39;
    }

    [S390xTest(40, "stdy", "Store Double (short displacement)")]
    public static int Test_stdy()
    {
        double a = 2.71828;
        return a == 2.71828 ? 0 : 40;
    }

    [S390xTest(41, "stmg", "Store Multiple (64-bit)")]
    public static int Test_stmg()
    {
        long a = 100L;
        long b = 200L;
        long c = 300L;
        return (a == 100L && b == 200L && c == 300L) ? 0 : 41;
    }

    [S390xTest(42, "lmg", "Load Multiple (64-bit)")]
    public static int Test_lmg()
    {
        long a = 111L;
        long b = 222L;
        long c = 333L;
        return (a == 111L && b == 222L && c == 333L) ? 0 : 42;
    }

    // ===== COMPARISON OPERATIONS (43-51) =====

    [S390xTest(43, "cr", "Compare Register (32-bit)")]
    public static int Test_cr()
    {
        int a = 10;
        int b = 10;
        int c = 5;
        return (a == b && a > c) ? 0 : 43;
    }

    [S390xTest(44, "cgr", "Compare Register (64-bit)")]
    public static int Test_cgr()
    {
        long a = 100L;
        long b = 100L;
        long c = 50L;
        return (a == b && a > c) ? 0 : 44;
    }

    [S390xTest(45, "clr", "Compare Logical Register (32-bit)")]
    public static int Test_clr()
    {
        uint a = 10U;
        uint b = 5U;
        return (a > b) ? 0 : 45;
    }

    [S390xTest(46, "clgr", "Compare Logical Register (64-bit)")]
    public static int Test_clgr()
    {
        ulong a = 100UL;
        ulong b = 50UL;
        return (a > b) ? 0 : 46;
    }

    [S390xTest(47, "chi", "Compare Halfword Immediate")]
    public static int Test_chi()
    {
        int a = 10;
        return (a == 10) ? 0 : 47;
    }

    [S390xTest(48, "cfi", "Compare Immediate (32-bit)")]
    public static int Test_cfi()
    {
        int a = 20;
        return (a > 10) ? 0 : 48;
    }

    [S390xTest(49, "cgfi", "Compare Immediate (64-bit)")]
    public static int Test_cgfi()
    {
        long a = 200L;
        return (a > 100L) ? 0 : 49;
    }

    [S390xTest(50, "clfi", "Compare Logical Immediate (32-bit)")]
    public static int Test_clfi()
    {
        uint a = 30U;
        return (a > 15U) ? 0 : 50;
    }

    [S390xTest(51, "clgfi", "Compare Logical Immediate (64-bit)")]
    public static int Test_clgfi()
    {
        ulong a = 300UL;
        return (a > 150UL) ? 0 : 51;
    }

    // ===== SHIFT OPERATIONS (52-55) =====

    [S390xTest(52, "srag", "Shift Right Arithmetic (64-bit)")]
    public static int Test_srag()
    {
        long a = -16L;
        return (a >> 2) == -4L ? 0 : 52;
    }

    [S390xTest(53, "sllg", "Shift Left Logical (64-bit)")]
    public static int Test_sllg()
    {
        long a = 8L;
        return (a << 3) == 64L ? 0 : 53;
    }

    [S390xTest(54, "srlg", "Shift Right Logical (64-bit)")]
    public static int Test_srlg()
    {
        ulong a = 0x8000000000000000UL;
        return (a >> 1) == 0x4000000000000000UL ? 0 : 54;
    }

    [S390xTest(55, "slag", "Shift Left Arithmetic (64-bit)")]
    public static int Test_slag()
    {
        long a = -1L;
        return (a << 3) == -8L ? 0 : 55;
    }

    // ===== FLOATING-POINT ARITHMETIC (56-63) =====

    [S390xTest(56, "aebr", "Add (short BFP)")]
    public static int Test_aebr()
    {
        float a = 3.5f;
        float b = 2.5f;
        return (a + b) == 6.0f ? 0 : 56;
    }

    [S390xTest(57, "adbr", "Add (long BFP)")]
    public static int Test_adbr()
    {
        double a = 3.5;
        double b = 2.5;
        return (a + b) == 6.0 ? 0 : 57;
    }

    [S390xTest(58, "sebr", "Subtract (short BFP)")]
    public static int Test_sebr()
    {
        float a = 10.0f;
        float b = 3.0f;
        return (a - b) == 7.0f ? 0 : 58;
    }

    [S390xTest(59, "sdbr", "Subtract (long BFP)")]
    public static int Test_sdbr()
    {
        double a = 10.0;
        double b = 3.0;
        return (a - b) == 7.0 ? 0 : 59;
    }

    [S390xTest(60, "meebr", "Multiply (short BFP)")]
    public static int Test_meebr()
    {
        float a = 4.0f;
        float b = 2.5f;
        return (a * b) == 10.0f ? 0 : 60;
    }

    [S390xTest(61, "mdbr", "Multiply (long BFP)")]
    public static int Test_mdbr()
    {
        double a = 4.0;
        double b = 2.5;
        return (a * b) == 10.0 ? 0 : 61;
    }

    [S390xTest(62, "debr", "Divide (short BFP)")]
    public static int Test_debr()
    {
        float a = 20.0f;
        float b = 4.0f;
        return (a / b) == 5.0f ? 0 : 62;
    }

    [S390xTest(63, "ddbr", "Divide (long BFP)")]
    public static int Test_ddbr()
    {
        double a = 20.0;
        double b = 4.0;
        return (a / b) == 5.0 ? 0 : 63;
    }

    // ===== FLOATING-POINT COMPARISON (64-65) =====

    [S390xTest(64, "cebr", "Compare (short BFP)")]
    public static int Test_cebr()
    {
        float a = 5.0f;
        float b = 3.0f;
        return (a > b) ? 0 : 64;
    }

    [S390xTest(65, "cdbr", "Compare (long BFP)")]
    public static int Test_cdbr()
    {
        double a = 5.0;
        double b = 3.0;
        return (a > b) ? 0 : 65;
    }

    // ===== CONVERSION OPERATIONS (66-75) =====

    [S390xTest(66, "cfebr", "Convert short BFP to 32-bit fixed")]
    public static int Test_cfebr()
    {
        float a = 5.7f;
        return (int)a == 5 ? 0 : 66;
    }

    [S390xTest(67, "cfdbr", "Convert long BFP to 32-bit fixed")]
    public static int Test_cfdbr()
    {
        double a = 5.7;
        return (int)a == 5 ? 0 : 67;
    }

    [S390xTest(68, "cgebr", "Convert short BFP to 64-bit fixed")]
    public static int Test_cgebr()
    {
        float a = 5.7f;
        return (long)a == 5L ? 0 : 68;
    }

    [S390xTest(69, "cgdbr", "Convert long BFP to 64-bit fixed")]
    public static int Test_cgdbr()
    {
        double a = 5.7;
        return (long)a == 5L ? 0 : 69;
    }

    [S390xTest(70, "clfebr", "Convert short BFP to 32-bit logical")]
    public static int Test_clfebr()
    {
        float a = 5.7f;
        return (uint)a == 5U ? 0 : 70;
    }

    [S390xTest(71, "clfdbr", "Convert long BFP to 32-bit logical")]
    public static int Test_clfdbr()
    {
        double a = 5.7;
        return (uint)a == 5U ? 0 : 71;
    }

    [S390xTest(72, "clgebr", "Convert short BFP to 64-bit logical")]
    public static int Test_clgebr()
    {
        float a = 5.7f;
        return (ulong)a == 5UL ? 0 : 72;
    }

    [S390xTest(73, "clgdbr", "Convert long BFP to 64-bit logical")]
    public static int Test_clgdbr()
    {
        double a = 5.7;
        return (ulong)a == 5UL ? 0 : 73;
    }

    [S390xTest(74, "ldebr", "Load Lengthened (short to long BFP)")]
    public static int Test_ldebr()
    {
        float  a    = 3.14f;
        double r    = (double)a;
        double diff = System.Math.Abs(r - 3.14);
        return diff <= 0.001 ? 0 : 74;
    }

    [S390xTest(75, "ledbr", "Load Rounded (long to short BFP)")]
    public static int Test_ledbr()
    {
        double a    = 3.14159;
        float  r    = (float)a;
        float  diff = System.Math.Abs(r - 3.14159f);
        return diff <= 0.001f ? 0 : 75;
    }

    // ===== DIVISION OPERATIONS (76-79) =====

    [S390xTest(76, "dr", "Divide (32-bit)")]
    public static int Test_dr()
    {
        int a = 100;
        int b = 5;
        return (a / b) == 20 ? 0 : 76;
    }

    [S390xTest(77, "dsgr", "Divide Single (64-bit)")]
    public static int Test_dsgr()
    {
        long a = 1000L;
        long b = 10L;
        return (a / b) == 100L ? 0 : 77;
    }

    [S390xTest(78, "dlr", "Divide Logical (32-bit)")]
    public static int Test_dlr()
    {
        uint a = 100U;
        uint b = 5U;
        return (a / b) == 20U ? 0 : 78;
    }

    [S390xTest(79, "dlgr", "Divide Logical (64-bit)")]
    public static int Test_dlgr()
    {
        ulong a = 1000UL;
        ulong b = 10UL;
        return (a / b) == 100UL ? 0 : 79;
    }

    // ===== BRANCH OPERATIONS (80-86) =====

    [S390xTest(80, "beq", "Branch if Equal")]
    public static int Test_beq()
    {
        int a = 10;
        int b = 10;
        return (a == b) ? 0 : 80;
    }

    [S390xTest(81, "bne", "Branch if Not Equal")]
    public static int Test_bne()
    {
        int a = 10;
        int b = 5;
        return (a != b) ? 0 : 81;
    }

    [S390xTest(82, "bgt", "Branch if Greater Than")]
    public static int Test_bgt()
    {
        int a = 10;
        int b = 5;
        return (a > b) ? 0 : 82;
    }

    [S390xTest(83, "ble", "Branch if Less or Equal")]
    public static int Test_ble()
    {
        int a = 5;
        int b = 10;
        return (a <= b) ? 0 : 83;
    }

    [S390xTest(84, "blt", "Branch if Less Than")]
    public static int Test_blt()
    {
        int a = 5;
        int b = 10;
        return (a < b) ? 0 : 84;
    }

    [S390xTest(85, "bge", "Branch if Greater or Equal")]
    public static int Test_bge()
    {
        int a = 10;
        int b = 5;
        return (a >= b) ? 0 : 85;
    }

    // ===== REGISTER OPERATIONS (87-90) =====

    [S390xTest(86, "lgr", "Load Register (64-bit)")]
    public static int Test_lgr()
    {
        long a = 12345L;
        long b = a;
        return b == 12345L ? 0 : 86;
    }

    [S390xTest(87, "llgfr", "Load Logical Register (32 to 64-bit zero-extend)")]
    public static int Test_llgfr()
    {
        int   a = -1;           // 0xFFFFFFFF
        ulong r = (ulong)(uint)a;
        return r == 0xFFFFFFFFUL ? 0 : 87;
    }

    [S390xTest(88, "lgfr", "Load Register (32 to 64-bit sign-extend)")]
    public static int Test_lgfr()
    {
        int  a = -1;
        long r = (long)a;
        return r == -1L ? 0 : 88;
    }

    [S390xTest(89, "lay", "Load Address")]
    public static int Test_lay()
    {
        int base_  = 5;
        int offset = 3;
        return (base_ + offset) == 8 ? 0 : 89;
    }
}
