﻿using InheritedMapper.Tests.SampleClasses;
using Nelibur.ObjectMapper;
using NUnit.Framework;

namespace InheritedMapper.Tests
{
    public class TargetClassResolverTest
    {
        [Test]
        public void TryCreate_ShouldReturnObject_WithPropertyValuesAsInSampleViewModelProperties()
        {
            var sampleViewModel = new SampleViewModel()
            {
                Guid = Guid.NewGuid(),
                SampleViewModelType = "DerivedSample",
                CreatedDate = new DateTime(2023, 01, 23, 23, 23, 23),
                ElementPrice = 1232.23m
            };

            var derivedSample = VmToInheritedModel.TryCreate<SampleViewModel, SampleBase>
                (sampleViewModel, sampleViewModel.SampleViewModelType, typeof(SampleBase));

            Assert.That(sampleViewModel.Guid == derivedSample.Guid);
            Assert.That(sampleViewModel.CreatedDate == new DateTime(2023, 01, 23, 23, 23, 23));
            Assert.That(sampleViewModel.ElementPrice == derivedSample.ElementPrice);
        }

        [Test]
        public void TryCreate_ShouldReturnObject_WithPropertyValuesAsInSampleViewModelProperties_2()
        {
            var sampleViewModel = new SampleViewModel()
            {
                Guid = Guid.NewGuid(),
                SampleViewModelType = "DerivedSecondSample",
                CreatedDate = new DateTime(2023, 01, 23, 23, 23, 23),
                ElementPrice = 1232.23m,
                OnlySecondDerivedSampleValue = "test value"
            };

            DerivedSecondSample derivedSecondSample = (DerivedSecondSample)VmToInheritedModel.TryCreate<SampleViewModel, SampleBase>
                (sampleViewModel, sampleViewModel.SampleViewModelType, typeof(SampleBase));

            Assert.That(sampleViewModel.Guid == derivedSecondSample.Guid);
            Assert.That(sampleViewModel.CreatedDate == new DateTime(2023, 01, 23, 23, 23, 23));
            Assert.That(sampleViewModel.ElementPrice == derivedSecondSample.ElementPrice);
            Assert.That(sampleViewModel.OnlySecondDerivedSampleValue == derivedSecondSample.OnlySecondDerivedSampleValue);
        }

        [Test]
        public void TryCreate_ShouldThrowDidNotFoundInheritedBaseClassException_IfWeAreNotAbleToFindAnyDerivedClassWhoHasNameLikeSpecifiedInViewModelProperty()
        {
            var targetDerivedClassTypeName = "DerivedSampleBlaBla";
            var sampleViewModel = new SampleViewModel() { Guid = Guid.NewGuid(), SampleViewModelType = targetDerivedClassTypeName };

            Assert.Throws<VmToInheritedModel.DidNotFoundInheritedBaseClassException>(() => VmToInheritedModel.TryCreate<SampleViewModel, SampleBase>
                (sampleViewModel, sampleViewModel.SampleViewModelType, typeof(SampleBase)));
        }

        [Test]
        public void TryCreate_ShouldCreateInctanceOfTypeSpecifiedInViewModelStringProperty_EvenIfBaseTypeHasAdditionalPropertiesOfTypeClasses()
        {
            var targetDerivedClassTypeName = "ExpandedBase";
            var sampleViewModel = new SampleViewModel() { Guid = Guid.NewGuid(), SampleViewModelType = targetDerivedClassTypeName };

            var result = VmToInheritedModel.TryCreate<SampleViewModel, ExpandedBaseAbstract>
                (sampleViewModel, sampleViewModel.SampleViewModelType, typeof(ExpandedBaseAbstract));

            Assert.That(result, Is.InstanceOf<ExpandedBase>());
        }

        [Test]
        public void TryCreate_ShouldCreateInctanceOfTypeSpecifiedInViewModelStringProperty_WithValuesConfiguredByTinyMapper()
        {
            var targetDerivedClassTypeName = "ExpandedBase";
            var sampleViewModel = new SampleViewModel() { Guid = Guid.NewGuid(), SampleViewModelType = targetDerivedClassTypeName };
            TinyMapper.Bind<SampleViewModel, ExpandedBase>(config =>
            {
                config.Bind(x => x.SampleViewModelType, x => x.ProductName);
            });

            var result = VmToInheritedModel.TryCreate<SampleViewModel, ExpandedBaseAbstract>
                (sampleViewModel, sampleViewModel.SampleViewModelType, typeof(ExpandedBaseAbstract));

            Assert.That(result.ProductName == targetDerivedClassTypeName);
            Assert.That(result.SampleBase == null);
        }

        [Test]
        public void TryCreate_ShouldThrowDidNotFoundBaseAbstractClassException_WhenTargetBaseClassIsNotAbstractClass()
        {
            var targetDerivedClassTypeName = "DerivedSample";
            var sampleViewModel = new SampleViewModel() { Guid = Guid.NewGuid(), SampleViewModelType = targetDerivedClassTypeName };

            Assert.Throws<VmToInheritedModel.DidNotFoundBaseAbstractClassException>(() => VmToInheritedModel.TryCreate<SampleViewModel, BaseNotAbstractClass>
                (sampleViewModel, sampleViewModel.SampleViewModelType, typeof(BaseNotAbstractClass)));
        }

        [Test]
        public void TryCreate_ShouldThrowException_WhenThereIsNoClassInSolutionWhichHasNameLikeSpecidedInViewModelProperty()
        {
            var targetDerivedClassTypeName = "DerivedSample";
            var sampleViewModel = new SampleViewModel() { Guid = Guid.NewGuid(), SampleViewModelType = targetDerivedClassTypeName };

            var derivedSample = VmToInheritedModel.TryCreate<SampleViewModel, SampleBase>
                (sampleViewModel, sampleViewModel.SampleViewModelType, typeof(SampleBase));

            Assert.That(derivedSample,Is.InstanceOf<DerivedSample>());
        }


    }
}