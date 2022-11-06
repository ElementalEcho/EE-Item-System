using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EE.Test.Util {
    public static class FluentAssertExtension {
        public static FluentAssert<T> Should<T>(this T value) {
            return new FluentAssert<T>(value);
        }

    }

    public class FluentAssert<T> {
        private readonly T value;
        public FluentAssert(T value) {
            this.value = value;
        }
        public void Be(T expected) {
            Assert.AreEqual(expected, value);
        }
        public void BeTrue() {
            Assert.AreEqual(true, value);
        }
        public void BeFalse() {
            Assert.AreEqual(false, value);
        }
        public void BeNull() {
            Assert.AreEqual(null, value);
        }
        public void BeNotNull() {
            Assert.AreNotEqual(null, value);
        }
    }
}