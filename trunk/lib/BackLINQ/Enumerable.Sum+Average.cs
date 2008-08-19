#region License, Terms and Author(s)
//
// BackLINQ
// Copyright (c) 2008 Atif Aziz. All rights reserved.
//
//  Author(s):
//
//      Atif Aziz, http://www.raboof.com
//
// This library is free software; you can redistribute it and/or modify it 
// under the terms of the New BSD License, a copy of which should have 
// been delivered along with this distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY 
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
#endregion

namespace System.Linq
{
    #region Imports

    using System;
    using System.Collections.Generic;

    #endregion

    partial class Enumerable
    {
        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Int32" /> values.
        /// </summary>

        public static int Sum(
            this IEnumerable<int> source)
        {
            CheckNotNull(source, "source");

            int sum = 0;
            foreach (var num in source)
                sum = checked(sum + num);

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Int32" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static int Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, int> selector)
        {
            return source.Select(selector).Sum();
        }
        
        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Int32" /> values.
        /// </summary>

        public static double Average(
            this IEnumerable<int> source)
        {
            CheckNotNull(source, "source");

            long sum = 0;
            long count = 0;

            foreach (var num in source)
            checked
            {
                sum += (int) num;
                count++;
            }

            if (count == 0)
                throw new InvalidOperationException();

            return (double) sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Int32" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static double Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, int> selector)
        {
            return source.Select(selector).Average();
        }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Int32" /> values.
        /// </summary>

        public static int? Sum(
            this IEnumerable<int?> source)
        {
            CheckNotNull(source, "source");

            int sum = 0;
            foreach (var num in source)
                sum = checked(sum + (num ?? 0));

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Int32" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static int? Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, int?> selector)
        {
            return source.Select(selector).Sum();
        }
        
        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Int32" /> values.
        /// </summary>

        public static double Average(
            this IEnumerable<int?> source)
        {
            CheckNotNull(source, "source");

            long sum = 0;
            long count = 0;

            foreach (var num in source.Where(n => n != null))
            checked
            {
                sum += (int) num;
                count++;
            }

            if (count == 0)
                throw new InvalidOperationException();

            return (double) sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Int32" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static double Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, int?> selector)
        {
            return source.Select(selector).Average();
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Int64" /> values.
        /// </summary>

        public static long Sum(
            this IEnumerable<long> source)
        {
            CheckNotNull(source, "source");

            long sum = 0;
            foreach (var num in source)
                sum = checked(sum + num);

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Int64" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static long Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, long> selector)
        {
            return source.Select(selector).Sum();
        }
        
        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Int64" /> values.
        /// </summary>

        public static double Average(
            this IEnumerable<long> source)
        {
            CheckNotNull(source, "source");

            long sum = 0;
            long count = 0;

            foreach (var num in source)
            checked
            {
                sum += (long) num;
                count++;
            }

            if (count == 0)
                throw new InvalidOperationException();

            return (double) sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Int64" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static double Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, long> selector)
        {
            return source.Select(selector).Average();
        }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Int64" /> values.
        /// </summary>

        public static long? Sum(
            this IEnumerable<long?> source)
        {
            CheckNotNull(source, "source");

            long sum = 0;
            foreach (var num in source)
                sum = checked(sum + (num ?? 0));

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Int64" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static long? Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, long?> selector)
        {
            return source.Select(selector).Sum();
        }
        
        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Int64" /> values.
        /// </summary>

        public static double Average(
            this IEnumerable<long?> source)
        {
            CheckNotNull(source, "source");

            long sum = 0;
            long count = 0;

            foreach (var num in source.Where(n => n != null))
            checked
            {
                sum += (long) num;
                count++;
            }

            if (count == 0)
                throw new InvalidOperationException();

            return (double) sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Int64" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static double Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, long?> selector)
        {
            return source.Select(selector).Average();
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Single" /> values.
        /// </summary>

        public static float Sum(
            this IEnumerable<float> source)
        {
            CheckNotNull(source, "source");

            float sum = 0;
            foreach (var num in source)
                sum = checked(sum + num);

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Single" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static float Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, float> selector)
        {
            return source.Select(selector).Sum();
        }
        
        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Single" /> values.
        /// </summary>

        public static float Average(
            this IEnumerable<float> source)
        {
            CheckNotNull(source, "source");

            float sum = 0;
            long count = 0;

            foreach (var num in source)
            checked
            {
                sum += (float) num;
                count++;
            }

            if (count == 0)
                throw new InvalidOperationException();

            return (float) sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Single" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static float Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, float> selector)
        {
            return source.Select(selector).Average();
        }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Single" /> values.
        /// </summary>

        public static float? Sum(
            this IEnumerable<float?> source)
        {
            CheckNotNull(source, "source");

            float sum = 0;
            foreach (var num in source)
                sum = checked(sum + (num ?? 0));

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Single" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static float? Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, float?> selector)
        {
            return source.Select(selector).Sum();
        }
        
        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Single" /> values.
        /// </summary>

        public static float Average(
            this IEnumerable<float?> source)
        {
            CheckNotNull(source, "source");

            float sum = 0;
            long count = 0;

            foreach (var num in source.Where(n => n != null))
            checked
            {
                sum += (float) num;
                count++;
            }

            if (count == 0)
                throw new InvalidOperationException();

            return (float) sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Single" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static float Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, float?> selector)
        {
            return source.Select(selector).Average();
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Double" /> values.
        /// </summary>

        public static double Sum(
            this IEnumerable<double> source)
        {
            CheckNotNull(source, "source");

            double sum = 0;
            foreach (var num in source)
                sum = checked(sum + num);

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Double" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static double Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, double> selector)
        {
            return source.Select(selector).Sum();
        }
        
        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Double" /> values.
        /// </summary>

        public static double Average(
            this IEnumerable<double> source)
        {
            CheckNotNull(source, "source");

            double sum = 0;
            long count = 0;

            foreach (var num in source)
            checked
            {
                sum += (double) num;
                count++;
            }

            if (count == 0)
                throw new InvalidOperationException();

            return (double) sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Double" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static double Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, double> selector)
        {
            return source.Select(selector).Average();
        }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Double" /> values.
        /// </summary>

        public static double? Sum(
            this IEnumerable<double?> source)
        {
            CheckNotNull(source, "source");

            double sum = 0;
            foreach (var num in source)
                sum = checked(sum + (num ?? 0));

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Double" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static double? Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, double?> selector)
        {
            return source.Select(selector).Sum();
        }
        
        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Double" /> values.
        /// </summary>

        public static double Average(
            this IEnumerable<double?> source)
        {
            CheckNotNull(source, "source");

            double sum = 0;
            long count = 0;

            foreach (var num in source.Where(n => n != null))
            checked
            {
                sum += (double) num;
                count++;
            }

            if (count == 0)
                throw new InvalidOperationException();

            return (double) sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Double" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static double Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, double?> selector)
        {
            return source.Select(selector).Average();
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Decimal" /> values.
        /// </summary>

        public static decimal Sum(
            this IEnumerable<decimal> source)
        {
            CheckNotNull(source, "source");

            decimal sum = 0;
            foreach (var num in source)
                sum = checked(sum + num);

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Decimal" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static decimal Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, decimal> selector)
        {
            return source.Select(selector).Sum();
        }
        
        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Decimal" /> values.
        /// </summary>

        public static decimal Average(
            this IEnumerable<decimal> source)
        {
            CheckNotNull(source, "source");

            decimal sum = 0;
            long count = 0;

            foreach (var num in source)
            checked
            {
                sum += (decimal) num;
                count++;
            }

            if (count == 0)
                throw new InvalidOperationException();

            return (decimal) sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Decimal" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static decimal Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, decimal> selector)
        {
            return source.Select(selector).Average();
        }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Decimal" /> values.
        /// </summary>

        public static decimal? Sum(
            this IEnumerable<decimal?> source)
        {
            CheckNotNull(source, "source");

            decimal sum = 0;
            foreach (var num in source)
                sum = checked(sum + (num ?? 0));

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Decimal" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static decimal? Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, decimal?> selector)
        {
            return source.Select(selector).Sum();
        }
        
        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Decimal" /> values.
        /// </summary>

        public static decimal Average(
            this IEnumerable<decimal?> source)
        {
            CheckNotNull(source, "source");

            decimal sum = 0;
            long count = 0;

            foreach (var num in source.Where(n => n != null))
            checked
            {
                sum += (decimal) num;
                count++;
            }

            if (count == 0)
                throw new InvalidOperationException();

            return (decimal) sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Decimal" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static decimal Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, decimal?> selector)
        {
            return source.Select(selector).Average();
        }
    }
}