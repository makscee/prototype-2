using System;

public class SequenceBuilder
{
    public SequenceRootDirectory directory; 
    public SequenceBuilder()
    {
        directory = new SequenceRootDirectory(this);
    }
    
    public void AddToChain(Interpolator<float> anim)
    {
        anim.Delay(_delayForNext);
        _delayForNext = 0f;
        if (_nextChainFront)
            anim.ObjectStackStart(this);
        else anim.ObjectStack(this);
        _nextChainFront = false;
    }

    float _delayForNext;
    bool _nextChainFront;
    public SequenceRootDirectory Chain(float delay = 0f)
    {
        _delayForNext = delay;
        return directory;
    }

    public SequenceRootDirectory With(float delay = 0f)
    {
        _delayForNext = delay;
        _nextChainFront = true;
        return directory;
    }

    public class SequenceRootDirectory
    {
        public ScreenSequenceDirectory Screen;
        public FieldSequenceDirectory Field;

        SequenceBuilder _builder;
        public SequenceRootDirectory(SequenceBuilder builder)
        {
            _builder = builder;
            Screen = new ScreenSequenceDirectory(builder);
        }

        public FieldSequenceDirectory FieldSet(FieldMatrix field)
        {
            Field = new FieldSequenceDirectory(_builder, field);
            return Field;
        }

        public SequenceRootDirectory Delay(float delay)
        {
            _builder._delayForNext = delay;
            return this;
        }

        public SequenceBuilder Lambda(Action a)
        {
            var anim = Animator.Interpolate(0f, 1f, 0f).WhenDone(a);
            _builder.AddToChain(anim);
            return _builder;
        }
    }
}