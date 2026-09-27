import { NextResponse } from 'next/server';
import { OnTheFlySettingsContext } from './add-framework'
import { useContext } from 'react';

export async function GET(
  ) {
    const context = useContext(OnTheFlySettingsContext);    
  
    return NextResponse.json(context.Current);
}

export async function PUT(
    request: Request
  ) {
    const newSettings = await request.json();
  
    const context = useContext(OnTheFlySettingsContext);
  
    context.replaceSettingsAsync(newSettings);

    return NextResponse.json({ message: 'Settings replaced', settings: newSettings }, { status: 201 });
  }