namespace Konamiman.Z80dotNet.Tests
{
    public class BitTests
    {
        [Test]
        public void Default_constructor_creates_instance_with_value_zero()
        {
            Assert.That(new Bit().Value == 0);
        }

        [Test]
        public void Can_implicitly_convert_to_int()
        {
            int theInt = new Bit(0);
            Assert.That(theInt == 0);

            theInt = new Bit(1);
            Assert.That(theInt, Is.EqualTo(1));
        }

        [Test]
        public void Can_implicitly_convert_from_zero_int_to_zero_bit()
        {
            Bit theBit = 0;
            Assert.That(0, Is.EqualTo(theBit.Value));
        }

        [Test]
        public void Can_implicitly_convert_from_any_nonzero_int_to_one_bit()
        {
            Bit theBit = 34;
            Assert.That(1, Is.EqualTo(theBit.Value));
        }

        [Test]
        public void Can_implicity_convert_to_and_from_bool()
        {
            Assert.That(new Bit(1) == 1);
            Assert.That(new Bit(0) == 0);

            Bit theTrue = true;
            Bit theFalse = false;
            Assert.That(theTrue.Value, Is.EqualTo(1));
            Assert.That(theFalse.Value == 0);
        }

        [Test]
        public void Can_create_one_instance_from_another()
        {
            Bit zero = 0;
            Bit one = 1;

            Assert.That(new Bit(zero) == 0);
            Assert.That(new Bit(one) == 1);
        }

        [Test]
        public void Can_compare_to_other_bit_with_equals_sign()
        {
            Assert.That(new Bit(0) == new Bit(0));
            Assert.That(new Bit(1) == new Bit(1));
            Assert.That((Bit)(new Bit(0) == new Bit(1)) == 0);
        }

        [Test]
        public void Can_compare_to_other_bit_with_equals_method()
        {
            Assert.That(new Bit(0).Equals(new Bit(0)));
            Assert.That(new Bit(1).Equals(new Bit(1)));
            Assert.That(new Bit(0) != new Bit(1));
        }

        [Test]
        public void Can_compare_for_equality_to_zero_int()
        {
            var zero = 0;
            var nonZero = 34;

            Assert.That(new Bit(0) == zero);
            Assert.That(new Bit(0).Equals(zero));

            Assert.That(zero == new Bit(0));
            Assert.That(zero.Equals(new Bit(0)));

            Assert.That(new Bit(0) != nonZero);
            Assert.That(!new Bit(0).Equals(nonZero));

            Assert.That(nonZero != new Bit(0));
            Assert.That(!nonZero.Equals(new Bit(0)));
        }

        [Test]
        public void Can_compare_for_equality_to_non_zero_int()
        {
            var zero = 0;
            var nonZero = 34;

            Assert.That(new Bit(1) == nonZero);
            Assert.That(new Bit(1) != zero);

            Assert.That(nonZero == new Bit(1));
            Assert.That(zero != new Bit(1));

            Assert.That(new Bit(1) == nonZero);
            Assert.That(!new Bit(1).Equals(zero));

            Assert.That(nonZero == new Bit(1));
            Assert.That(!nonZero.Equals(new Bit(1)));

        }

        [Test]
        public void Can_compare_for_inequality_to_zero_int()
        {
            var zero = 0;
            var nonZero = 34;

            Assert.That(new Bit(1) == nonZero);
            Assert.That(new Bit(1) != zero);
            Assert.That(zero != new Bit(1));

            Assert.That(new Bit(1) == nonZero);
            Assert.That(nonZero == new Bit(1));
        }

        [Test]
        public void Can_convert_to_bool()
        {
            Assert.That(new Bit(0) == 0);
            Assert.That(new Bit(1));
        }

        [Test]
        public void Can_do_arithmetic_OR()
        {
            Bit zero = 0;
            Bit one = 1;

            Assert.That(zero | zero == 0);
            Assert.That(zero | one);
            Assert.That(one | zero);
            Assert.That(one | one);
        }

        [Test]
        public void Can_do_logical_OR()
        {
            Bit zero = 0;
            Bit one = 1;

            Assert.That((zero || zero) == 0);
            Assert.That(zero || one);
            Assert.That(one || zero);
            Assert.That(one || one);
        }

        [Test]
        public void Can_do_arithmetic_AND()
        {
            Bit zero = 0;
            Bit one = 1;

            Assert.That((zero & zero) == 0);
            Assert.That((zero & one) == 0);
            Assert.That((one & zero) == 0);
            Assert.That(one & one);
        }

        [Test]
        public void Can_do_logical_AND()
        {
            Bit zero = 0;
            Bit one = 1;

            Assert.That((zero && zero) == 0);
            Assert.That((zero && one) == 0);
            Assert.That((one && zero) == 0);
            Assert.That(one && one);
        }

        [Test]
        public void Can_do_arithmetic_XOR()
        {
            Bit zero = 0;
            Bit one = 1;

            Assert.That(zero ^ zero == 0);
            Assert.That(zero ^ one);
            Assert.That(one ^ zero);
            Assert.That(one ^ one == 0);
        }

        [Test]
        public void Can_do_arithmetic_NOT()
        {
            Bit zero = 0;
            Bit one = 1;

            Assert.That(~one == 0);
            Assert.That(~zero == one);
        }

        [Test]
        public void Can_do_logical_NOT()
        {
            Bit zero = 0;
            Bit one = 1;

            Assert.That(!one == 0);
            Assert.That(!zero == one);
        }
    }
}
